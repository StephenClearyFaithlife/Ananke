#addin "Cake.Git"
#addin nuget:?package=Cake.XmlDocMarkdown&version=1.4.0

using System.Text.RegularExpressions;

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var trigger = Argument("trigger", "");
var versionSuffix = Argument("versionSuffix", "");

var nugetApiKey = EnvironmentVariable("NUGETAPIKEY");

var solutionFileName = GetFiles("*.sln").Single().GetFilename().ToString();
var docsProjects = GetFiles("src/**/*.csproj").Select(x => x.GetFilenameWithoutExtension()).ToArray();
var docsRepoUri = "https://github.com/Faithlife/Ananke.git";
var docsSourceUri = "https://github.com/Faithlife/Ananke/tree/master/src";

var nugetSource = "https://api.nuget.org/v3/index.json";
var buildBotUserName = "faithlifebuildbot";
var buildBotPassword = EnvironmentVariable("BUILD_BOT_PASSWORD");

Task("Clean")
	.Does(() =>
	{
		CleanDirectories("src/**/bin");
		CleanDirectories("src/**/obj");
		CleanDirectories("test/**/bin");
		CleanDirectories("test/**/obj");
		CleanDirectories("deploy");
	});

Task("Restore")
	.Does(() =>
	{
		DotNetCoreRestore(solutionFileName);
	});

Task("Build")
	.IsDependentOn("Restore")
	.Does(() =>
	{
		DotNetCoreBuild(solutionFileName, new DotNetCoreBuildSettings { Configuration = configuration, NoRestore = true, ArgumentCustomization = args => args.Append("--verbosity normal") });
	});

Task("Rebuild")
	.IsDependentOn("Clean")
	.IsDependentOn("Build");

Task("UpdateDocs")
	.WithCriteria(!string.IsNullOrEmpty(buildBotPassword))
	.WithCriteria(EnvironmentVariable("APPVEYOR_REPO_BRANCH") == "master")
	.IsDependentOn("Build")
	.Does(() =>
	{
		var branchName = "gh-pages";
		var docsDirectory = new DirectoryPath(branchName);
		GitClone(docsRepoUri, docsDirectory, new GitCloneSettings { BranchName = branchName });

		Information($"Updating documentation at {docsDirectory}.");
		foreach (var docsProject in docsProjects)
		{
			XmlDocMarkdownGenerate(GetFiles($"src/{docsProject}/bin/{configuration}/*/{docsProject}.dll").Single().ToString(), $"{docsDirectory}{System.IO.Path.DirectorySeparatorChar}",
				new XmlDocMarkdownSettings { SourceCodePath = $"{docsSourceUri}/{docsProject}", NewLine = "\n", ShouldClean = true });
		}

		if (GitHasUncommitedChanges(docsDirectory))
		{
			Information("Committing all documentation changes.");
			GitAddAll(docsDirectory);
			GitCommit(docsDirectory, "Faithlife Build Bot", "faithlifebuildbot@users.noreply.github.com", "Automatic documentation update.");
			Information("Pushing updated documentation to GitHub.");
			GitPush(docsDirectory, buildBotUserName, buildBotPassword, branchName);
		}
		else
		{
			Information("No documentation changes detected.");
		}
	});

Task("Test")
	.IsDependentOn("Build")
	.Does(() =>
	{
		foreach (var projectPath in GetFiles("test/**/*.csproj").Select(x => x.FullPath))
			DotNetCoreTest(projectPath, new DotNetCoreTestSettings { Configuration = configuration, NoBuild = true, NoRestore = true });
	});

Task("NuGetPackage")
	.IsDependentOn("Rebuild")
	.IsDependentOn("Test")
	.IsDependentOn("UpdateDocs")
	.Does(() =>
	{
		if (string.IsNullOrEmpty(versionSuffix) && !string.IsNullOrEmpty(trigger))
			versionSuffix = Regex.Match(trigger, @"^v[^\.]+\.[^\.]+\.[^\.]+-(.+)").Groups[1].ToString();
		foreach (var projectPath in GetFiles("src/**/*.csproj").Select(x => x.FullPath))
			DotNetCorePack(projectPath, new DotNetCorePackSettings { Configuration = configuration, NoBuild = true, NoRestore = true, OutputDirectory = "deploy", VersionSuffix = versionSuffix });
	});

Task("NuGetPackageTests")
	.IsDependentOn("NuGetPackage")
	.Does(() =>
	{
		var firstProject = GetFiles("src/**/*.csproj").First().FullPath;
		foreach (var nupkg in GetFiles("deploy/**/*.nupkg").Select(x => x.FullPath))
			DotNetCoreTool(firstProject, "sourcelink", $"test {nupkg}");
	});

Task("NuGetPublish")
	.IsDependentOn("NuGetPackageTests")
	.Does(() =>
	{
		var nupkgPaths = GetFiles("deploy/*.nupkg").Select(x => x.FullPath).ToList();

		string version = null;
		foreach (var nupkgPath in nupkgPaths)
		{
			string nupkgVersion = Regex.Match(nupkgPath, @"\.([^\.]+\.[^\.]+\.[^\.]+)\.nupkg$").Groups[1].ToString();
			if (version == null)
				version = nupkgVersion;
			else if (version != nupkgVersion)
				throw new InvalidOperationException($"Mismatched package versions '{version}' and '{nupkgVersion}'.");
		}

		if (!string.IsNullOrEmpty(nugetApiKey) && (trigger == null || Regex.IsMatch(trigger, "^v[0-9]")))
		{
			if (trigger != null && trigger != $"v{version}")
				throw new InvalidOperationException($"Trigger '{trigger}' doesn't match package version '{version}'.");

			var pushSettings = new NuGetPushSettings { ApiKey = nugetApiKey, Source = nugetSource };
			foreach (var nupkgPath in nupkgPaths)
				NuGetPush(nupkgPath, pushSettings);
		}
		else
		{
			Information("To publish this package, push this git tag: v" + version);
		}
	});

Task("Default")
	.IsDependentOn("NuGetPublish");

RunTarget(target);
