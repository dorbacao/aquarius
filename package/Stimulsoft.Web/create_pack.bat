cd ..
cd ..
cd ".nuget"

set path=%path%;%CD%

cd ..

cd Packages
cd Stimulsoft.Web

nuget pack Stimulsoft.Web.nuspec
copy Stimulsoft.Web*.nupkg Output\
del Stimulsoft.Web*.nupkg