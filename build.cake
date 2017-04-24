var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var configuration = Argument("configuration", "Debug");
// Define directories.
var buildDir = Directory("./Dragonfly/bin");// + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("Clean")
	.Does(()=>
{
	CleanDirectory(buildDir);
});

Task("Restore-nuget")
    .IsDependentOn("Clean")
	.Does(()=>
{
	#break
	NuGetRestore("./Dragonfly.sln");
});

Task("Build")
	.IsDependentOn("Restore-nuget")
	.Does(()=>
{
	MSBuild("./Dragonfly.sln");
});

Task("Default").
	IsDependentOn("Build");

RunTarget("Default");