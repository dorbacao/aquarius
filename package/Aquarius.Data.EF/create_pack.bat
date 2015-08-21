cd ..
cd ..
cd ".nuget"

set path=%path%;%CD%

cd ..

cd package
cd Aquarius.Data.EF

nuget pack Aquarius.Data.EF.nuspec
copy Aquarius.Data.EF*.nupkg Output\
del Aquarius.Data.EF*.nupkg