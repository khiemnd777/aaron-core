@echo off
NuGet.exe setApiKey 87835a82-0fe7-4a95-a810-bc2445803f26
NuGet.exe pack Nuget\Aaron.Core.nuspec
NuGet.exe pack Nuget\Aaron.Data.nuspec
NuGet.exe pack Nuget\Aaron.MVC.nuspec
NuGet.exe pack Nuget\Aaron.Registrar.nuspec