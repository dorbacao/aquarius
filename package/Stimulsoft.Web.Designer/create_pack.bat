@echo off
..\..\.nuget\nuget.exe pack Stimulsoft.Web.Designer.nuspec
move Stimulsoft.Web*.nupkg Output\
@echo on