/// <binding />
var gulp = require("gulp");
var msbuild = require("gulp-msbuild");
var debug = require("gulp-debug");
var foreach = require("gulp-foreach");
var rename = require("gulp-rename");
var watch = require("gulp-watch");
var merge = require("merge-stream");
var newer = require("gulp-newer");
var util = require("gulp-util");
var runSequence = require("run-sequence");
var path = require("path");
var config = require("./gulp-config.js")();
var nugetRestore = require("gulp-nuget-restore");
var fs = require("fs");
var yargs = require("yargs").argv;
var cssmin = require("gulp-cssmin");
var rename = require("gulp-rename");
var concat = require("gulp-concat");
var uglify = require("gulp-uglify");
module.exports.config = config;

gulp.task("default", function (callback) {
    config.runCleanBuilds = true;
    return runSequence(
        "01-Copy-Sitecore-License",
        "02-Nuget-Restore",
        "03-Publish-All-Projects",
        "04-Apply-Xml-Transform",
        //"05-Sync-Unicorn",
        "06-Deploy-Transforms",
        callback);
});

gulp.task("deploy", function (callback) {
    config.runCleanBuilds = true;
    return runSequence(
        "01-Copy-Sitecore-License",
        "02-Nuget-Restore",
        "03-Publish-All-Projects",
        "04-Apply-Xml-Transform",
        callback);
});

gulp.task("Publish-Custom", function (callback) {
    config.runCleanBuilds = true;
    return runSequence(
        "Publish-All-Configs",
        "Publish-All-Fonts",
        //"minify-css",
        //"minify-jss",
        "Publish-All-Views",
        //"Publish-All-Scripts",
        "Publish-Assemblies",
        "Publish-Dependencies",
        callback);
});

/*****************************
  Initial setup
*****************************/
gulp.task("01-Copy-Sitecore-License", function () {
    console.log("Copying Sitecore License file");

    return gulp.src(config.licensePath).pipe(gulp.dest("./lib"));
});

gulp.task("02-Nuget-Restore", function (callback) {
    var solution = "./" + config.solutionName + ".sln";
    return gulp.src(solution).pipe(nugetRestore());
});


gulp.task("03-Publish-All-Projects", function (callback) {
    return runSequence(
        "Build-Solution",
        "Publish-Foundation-Projects",
        "Publish-Feature-Projects",
        "Publish-Project-Projects", callback);
});

gulp.task("04-Apply-Xml-Transform", function () {
    var layerPathFilters = ["./Foundation/**/*.transform", "./Feature/**/*.transform", "./Website/**/*.transform", "!./**/obj/**/*.transform", "!./**/bin/**/*.transform"];
    return gulp.src(layerPathFilters)
        .pipe(foreach(function (stream, file) {
            var fileToTransform = file.path.replace(/.+code\\(.+)\.transform/, "$1");
            util.log("Applying configuration transform: " + file.path);
            return gulp.src("./scripts/applytransform.targets")
                .pipe(msbuild({
                    targets: ["ApplyTransform"],
                    configuration: config.buildConfiguration,
                    logCommand: false,
                    verbosity: "minimal",
                    stdout: true,
                    errorOnFail: true,
                    maxcpucount: 0,
                    toolsVersion: config.buildToolsVersion,
                    properties: {
                        Platform: config.buildPlatform,
                        WebConfigToTransform: config.websiteRoot,
                        TransformFile: file.path,
                        FileToTransform: fileToTransform
                    }
                }));
        }));
});

/*gulp.task("05-Sync-Unicorn", function (callback) {
    var options = {};
    options.siteHostName = habitat.getSiteUrl();
    options.authenticationConfigFile = config.websiteRoot + "/App_config/Include/Unicorn/Unicorn.UI.config";

    unicorn(function () { return callback() }, options);
});*/


gulp.task("06-Deploy-Transforms", function () {
    return gulp.src("./**/code/**/*.transform")
        .pipe(gulp.dest(config.websiteRoot + "/temp/transforms"));
});

/*****************************
  Copy assemblies to all local projects
*****************************/
gulp.task("Copy-Local-Assemblies", function () {
    console.log("Copying site assemblies to all local projects");
    var files = config.sitecoreLibraries + "/**/*";

    var root = "./";
    var projects = root + "/**/code/bin";
    return gulp.src(projects, { base: root })
        .pipe(foreach(function (stream, file) {
            console.log("copying to " + file.path);
            gulp.src(files)
                .pipe(gulp.dest(file.path));
            return stream;
        }));
});

/*****************************
  Publish
*****************************/
var publishStream = function (stream, dest) {
    var targets = ["Build"];
    return stream
        .pipe(debug({ title: "Building project:" }))
        .pipe(msbuild({
            targets: targets,
            configuration: config.buildConfiguration,
            logCommand: true,
            verbosity: "minimal",
            stdout: true,
            errorOnFail: true,
            maxcpucount: 0,
            toolsVersion: config.buildToolsVersion,
            properties: {
                Platform: config.publishPlatform,
                DeployOnBuild: "true",
                DeployDefaultTarget: "WebPublish",
                WebPublishMethod: "FileSystem",
                DeleteExistingFiles: "false",
                publishUrl: dest,
                _FindDependencies: "false"
            }
        }));
}

var publishProject = function (location, dest) {
    dest = dest || config.websiteRoot;

    console.log("publish to " + dest + " folder" + " from " + location);
    return gulp.src(["./" + location + "/*.csproj"])
        .pipe(foreach(function (stream, file) {
            return publishStream(stream, dest);
        }));
}

var publishProjects = function (location, dest) {
    dest = dest || config.websiteRoot;

    console.log("publish to " + dest + " folder" + " from " + location);
    return gulp.src([config.solutionName + "." + location + "*/*.csproj"])
        .pipe(foreach(function (stream, file) {
            return publishStream(stream, dest);
        }));
};

gulp.task("Build-Solution", function () {
    var targets = ["Build"];
    if (config.runCleanBuilds) {
        targets = ["Clean", "Build"];
    }

    var solution = "./" + config.solutionName + ".sln";
    return gulp.src(solution)
        .pipe(msbuild({
            targets: targets,
            configuration: config.buildConfiguration,
            logCommand: false,
            verbosity: "minimal",
            stdout: true,
            errorOnFail: true,
            maxcpucount: 0,
            toolsVersion: config.buildToolsVersion,
            properties: {
                Platform: config.buildPlatform
            }
        }));
});

gulp.task("Publish-Foundation-Projects", function () {
    return publishProjects("Foundation");
});

gulp.task("Publish-Feature-Projects", function () {
    return publishProjects("Feature");
});

gulp.task("Publish-Project-Projects", function () {
    return publishProjects("Website");
});

gulp.task("Publish-Project", function () {
    if (yargs && yargs.m && typeof (yargs.m) === 'string') {
        return publishProject(yargs.m);
    } else {
        throw "\n\n------\n USAGE: -m Layer/Module \n------\n\n";
    }
});

gulp.task("Publish-Assemblies", function () {
    var root = "./";
    var binFiles = root + "/src/**/**/**/bin/HackathonWeb.{Feature,Foundation,Website}*.{dll,pdb}";
    var destination = config.websiteRoot + "/bin/";
    return gulp.src(binFiles, { base: root })
        .pipe(rename({ dirname: "" }))
        .pipe(newer(destination))
        .pipe(debug({ title: "Copying " }))
        .pipe(gulp.dest(destination));
});

gulp.task("Publish-Dependencies", function () {
    var root = "./";
    var binFiles = root + "/src/**/**/**/bin/{Glass,Castle,SimpleInjector,DynamicPlaceholders}*.{dll,pdb}";
    var destination = config.websiteRoot + "/bin/";
    return gulp.src(binFiles, { base: root })
        .pipe(rename({ dirname: "" }))
        .pipe(newer(destination))
        .pipe(debug({ title: "Copying " }))
        .pipe(gulp.dest(destination));
});

gulp.task("Publish-All-Views", function () {
    var root = "./";
    var roots = [root + "/**/Views", "!" + root + "/**/obj/**/Views"];
    var files = "/**/*.cshtml";
    var destination = config.websiteRoot + "\\Views";
    return gulp.src(roots, { base: root }).pipe(
        foreach(function (stream, file) {
            console.log("Publishing from " + file.path);
            gulp.src(file.path + files, { base: file.path })
                .pipe(newer(destination))
                .pipe(debug({ title: "Copying " }))
                .pipe(gulp.dest(destination));
            return stream;
        })
    );
});

gulp.task("Publish-All-Scripts", function () {
    var root = "./";
    var roots = [root + "/**/Scripts"];
    var files = "/*.js";
    var destination = config.websiteRoot + "\\Scripts";
    return gulp.src(roots, { base: root }).pipe(
        foreach(function (stream, file) {
            console.log("Publishing from " + file.path);
            gulp.src(file.path + files, { base: file.path })
                .pipe(newer(destination))
                .pipe(debug({ title: "Copying " }))
                .pipe(gulp.dest(destination));
            return stream;
        })
    );
});

gulp.task("Publish-All-Fonts", function () {
    var root = "./";
    var roots = [root + "/**/fonts"];
    var files = "/*.css";
    var destination = config.websiteRoot + "\\fonts";
    return gulp.src(roots, { base: root }).pipe(
        foreach(function (stream, file) {
            console.log("Publishing from " + file.path);
            gulp.src(file.path + files, { base: file.path })
                .pipe(newer(destination))
                .pipe(debug({ title: "Copying " }))
                .pipe(gulp.dest(destination));
            return stream;
        })
    );
});

gulp.task("Publish-All-Configs", function () {
    var root = "./";
    var roots = [root + "/**/App_Config", "!" + root + "/**/obj/**/App_Config"];
    var files = "/**/*.config";
    var destination = config.websiteRoot + "\\App_Config";
    return gulp.src(roots, { base: root }).pipe(
        foreach(function (stream, file) {
            console.log("Publishing from " + file.path);
            gulp.src(file.path + files, { base: file.path })
                .pipe(newer(destination))
                .pipe(debug({ title: "Copying " }))
                .pipe(gulp.dest(destination));
            return stream;
        })
    );
});


/*****************************
 Watchers
*****************************/
gulp.task("Auto-Publish-Css", function () {
    var root = "./";
    var roots = [root + "/**/Content"];
    var files = "/*.css";
    var destination = config.websiteRoot + "\\Content";
    gulp.src(roots, { base: root }).pipe(
        foreach(function (stream, rootFolder) {
            gulp.watch(rootFolder.path + files, function (event) {
                if (event.type === "changed") {
                    console.log("publish this file " + event.path);
                    gulp.src(event.path, { base: rootFolder.path }).pipe(gulp.dest(destination));
                }
                console.log("published " + event.path);
            });
            return stream;
        })
    );
});

gulp.task("Auto-Publish-Views", function () {
    var root = "./";
    var roots = [root + "/**/Views", "!" + root + "/**/obj/**/Views"];
    var files = "/**/*.cshtml";
    var destination = config.websiteRoot + "\\Views";
    return gulp.src(roots, { base: root }).pipe(
        foreach(function (stream, rootFolder) {
            gulp.watch(rootFolder.path + files, function (event) {
                if (event.type === "changed") {
                    console.log("publish this file " + event.path);
                    gulp.src(event.path, { base: rootFolder.path }).pipe(gulp.dest(destination));
                }
                console.log("published " + event.path);
            });
            return stream;
        })
    );
});

gulp.task("Auto-Publish-Assemblies", function () {
    var root = "./";
    var roots = [root + "/**/bin"];
    var files = "/**/HackathonWeb.{Feature,Foundation,Website}.*.{dll,pdb}";
    var destination = config.websiteRoot + "/bin/";
    gulp.src(roots, { base: root }).pipe(
        foreach(function (stream, rootFolder) {
            gulp.watch(rootFolder.path + files, function (event) {
                if (event.type === "changed") {
                    console.log("publish this file " + event.path);
                    gulp.src(event.path, { base: rootFolder.path }).pipe(gulp.dest(destination));
                }
                console.log("published " + event.path);
            });
            return stream;
        })
    );
});

var minifyCss = function (destination) {
    console.log("css to " + destination + " folder");
    return gulp.src("./**/Content/**/{jquery-ui,bootstrap,bootstrap-theme,hackathonweb.website,foundation.sitecoreextensions}.css")
        .pipe(concat('hackathonweb.website.min.css'))
        .pipe(cssmin())
        .pipe(debug({ title: "Copying " }))
        .pipe(gulp.dest(destination));
};

var minifyJs = function (destination) {
    console.log("js to " + destination + " folder");
    return gulp.src("./**/Scripts/{jquery-1.12.4,jquery-ui-1.12.1,bootstrap}.js")
        .pipe(concat('hackathonweb.website.min.js'))
        .pipe(uglify())
        .pipe(debug({ title: "Copying " }))
        .pipe(gulp.dest(destination));
};

gulp.task("minify-css", function () {
    return minifyCss(config.websiteRoot + "\\Content");
});

gulp.task("minify-js", function () {
    return minifyJs(config.websiteRoot + "\\Scripts");
});

gulp.task("css-watcher", function () {
    var root = "./";
    var roots = [root + "**/Content", "!" + root + "/**/obj/**/Content"];
    var files = "/**/*.css";
    var destination = config.webRoot + "\\Content";
    gulp.src(roots, { base: root }).pipe(
        foreach(function (stream, rootFolder) {
            gulp.watch(rootFolder.path + files, function (event) {
                if (event.type === "changed") {
                    console.log("publish this file " + event.path);
                    minifyCss(destination);
                }
                console.log("published " + event.path);
            });
            return stream;
        })
    );
});

gulp.task("js-watcher", function () {
    var root = "./";
    var roots = [root + "**/Scripts", "!" + root + "/**/obj/**/Scripts"];
    var files = "/**/*.js";
    var destination = config.webRoot + "\\Scripts";
    gulp.src(roots, { base: root }).pipe(
        foreach(function (stream, rootFolder) {
            gulp.watch(rootFolder.path + files, function (event) {
                if (event.type === "changed") {
                    console.log("publish this file " + event.path);
                    minifyJs(destination);
                }
                console.log("published " + event.path);
            });
            return stream;
        })
    );
});