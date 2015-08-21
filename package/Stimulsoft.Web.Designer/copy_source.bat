@echo off
set sourceFolder=..\..\source\bin
set destinationFolder=..\..\Packages\Stimulsoft.Web.Designer\lib

copy %sourceFolder%\Stimulsoft.Base.dll %destinationFolder%
copy %sourceFolder%\Stimulsoft.Controls.Win.dll %destinationFolder%
copy %sourceFolder%\Stimulsoft.Controls.dll %destinationFolder%
copy %sourceFolder%\Stimulsoft.Editor.dll %destinationFolder%
copy %sourceFolder%\Stimulsoft.Report.dll %destinationFolder%
copy %sourceFolder%\Stimulsoft.Report.Check.dll %destinationFolder%
copy %sourceFolder%\Stimulsoft.Report.Design.dll %destinationFolder%
copy %sourceFolder%\Stimulsoft.Report.Helper.dll %destinationFolder%
copy %sourceFolder%\Stimulsoft.Report.Web.dll %destinationFolder%
copy %sourceFolder%\Stimulsoft.Report.WebDesign.dll %destinationFolder%
@echo on