cd ..
cd ..
cd ".nuget"

set path=%path%;%CD%

cd ..

cd package
cd Aquarius.Seedwork

nuget pack Aquarius.Seedwork.nuspec
copy Aquarius.Seedwork*.nupkg Output\
del Aquarius.Seedwork*.nupkg