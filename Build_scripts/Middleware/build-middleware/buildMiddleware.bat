@echo off
rem Builds the Middleware project.
rem Usage: buildMiddleware.bat version
rem 
rem Version is Major.minor.build.revision. Example 1.1.101713.0, build is date, revision is build # for that day.

rem Version scheme
rem Major.minor.build.revision
rem Major - Assemblies with the same name but different major versions are not interchangeable. A higher version number might indicate a major rewrite of a product where backward compatibility cannot be assumed.
rem Minor - If the name and major version number on two assemblies are the same, but the minor version number is different, this indicates significant enhancement with the intention of backward compatibility. 
rem Build - see below XXYY, XX is number of months since January 2013. YY is day of month.
rem Revision - Build # of the day.
rem
rem Build notes: made up of:
rem Year in which project started (13). January is month 1. First 2 digits of build number are number of months sinse month 1. Last 2 digits are day of month.
rem So October 10/18/13 is build# 1018



rem Versioning
rem Because Middleware spans GIT repositories, versioning is a little tricky.
rem Versioning plan:
rem 	1. Build script takes as input a version Major.minor.build.revision
rem		2. Build script gets git commit# from multiple repositories.
rem 	3. Build script writes assembly version files for each app and middleware, which is included in Middleware application projects.
rem 	4. Code gets built (code is modified to log Major.minor.build.revision).
rem 	[Not implemented] 5. Build script tags repositories with Major.minor.build.revision
rem		[Not implemented, track manually] 6. Build script updates a file with Major.minor.build.revision and git repository commit numbers and commits this file.
rem		7. Version (Major.minor.build.revision) gets put into installer filename and installer version that you see in add/remove programs.



rem count the args
SET /A ARGS_COUNT=0    
FOR %%A in (%*) DO SET /A ARGS_COUNT+=1    
rem ECHO %ARGS_COUNT%

if %ARGS_COUNT% NEQ 1 (
	echo Usage: buildMiddleware.bat version
	echo Version is major.minor.build.revision. Example 1.01.1021.0, build is XXYY, XX is month# begining Jan 2013, YY is day of month, revision is build # for that day.
	GOTO:EOF
)

IF DEFINED MSBuildPath 	(
	ECHO MSBuildPath IS defined
	) ELSE (
	ECHO MSBuildPath is NOT defined
	set MSBuildPath="C:\Windows\Microsoft.NET\Framework\v4.0.30319"
	echo using %MSBuildPath%
	)
	
IF DEFINED sourceRoot 	(
	ECHO sourceRoot IS defined
	) ELSE (
	ECHO sourceRoot is NOT defined
	set sourceRoot=C:\Users\bhuzyk\Documents\GitHub
	echo using %sourceRoot%
	)	

IF DEFINED devenv10Path 	(
	ECHO devenv10Path IS defined
	) ELSE (
	ECHO devenv10Path is NOT defined
	set devenv10Path="C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\"
	echo using %devenv10Path%
	)		
	
rem MSBuild task ddl folder	
set taskDllFolder=%sourceRoot%\ImageGen\Build_scripts\dlls\
set generated_msi_folder=%sourceRoot%\ImageGen\Build_scripts\Middleware\build-middleware\setup\MiddlewareSetup\Generated_msi
	
rem Get configuration (Debug/Release) from commandline
set Configuration=Debug

set version=%1

%MSBuildPath%\MSBuild.exe buildMiddleware.proj 