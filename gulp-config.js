module.exports = function () {
    var instanceRoot = "C:\\inetpub\\wwwroot\\sc910.sc";
    var config = {
        websiteRoot: instanceRoot,
        sitecoreLibraries: instanceRoot + "\\bin",
        licensePath: instanceRoot + "\\App_Data\\license.xml",
        solutionName: "Hackathon.Boilerplate",
        buildConfiguration: "Debug",
        buildPlatform: "Any CPU",
        buildToolsVersion: 14.0, //change to 15.0 for VS2017 support
        publishPlatform: "AnyCpu",
        runCleanBuilds: false
    };
    return config;
}