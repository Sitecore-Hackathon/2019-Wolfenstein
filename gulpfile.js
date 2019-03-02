/// <binding />
var gulp = require("gulp");
var msbuild = require("gulp-msbuild");
var debug = require("gulp-debug");
var foreach = require("gulp-foreach");
var rename = require("gulp-rename");
var newer = require("gulp-newer");
var runSequence = require("run-sequence");
var config = require("./gulp-config.js")();
module.exports.config = config;

gulp.task("Publish-Custom", function (callback) {
    config.runCleanBuilds = true;
    return runSequence(
        "Publish-All-Configs",
        "Publish-All-Views",
        "Publish-All-Scripts",
        "Publish-Assemblies",
        "Publish-Dependencies",
        callback);
});

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
    var binFiles = root + "/src/**/**/**/bin/{Microsoft.Extensions.DependencyInjection,Glass,Castle,SimpleInjector,DynamicPlaceholders,Accord}*.{dll,pdb}";
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