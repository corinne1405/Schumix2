#!/bin/sh

echo "Running autogen.sh"
sh autogen.sh
echo "Running configure"
./configure --prefix=/usr
echo "Running make"
make

rm -rf package
mkdir package
cp -rf debian package/
mkdir package/usr
mkdir package/usr/bin
mv package/debian/schumix package/usr/bin/
mv package/debian/schumix-server package/usr/bin/
mv package/debian/share package/usr/
mkdir package/usr/lib
mkdir package/usr/lib/pkgconfig
cd Run/Release/Addons

for file in *.pc
do
	echo $file;
	mv $file ../../../package/usr/lib/pkgconfig/$file
done

cd ..

for file in *.pc
do
	echo $file;
	mv $file ../../package/usr/lib/pkgconfig/$file
done

rm Config.exe Installer.exe Addons/Schumix.db3 Addons/sqlite3.dll Addons/System.Data.SQLite.dll Addons/MySql.Data.dll Addons/Schumix.Irc.dll Addons/Schumix.API.dll Addons/Schumix.Framework.dll schumix.config schumix.installer schumix schumix.server
cp -rf ./ ../../package/usr/lib/schumix
cd ../../
cd package
#control
cd debian
revision="$(echo $(cat ../../Core/Schumix.Framework/Config/Consts.cs) | grep -o 'SchumixVersion = ".*"; public const string SchumixFileVersion' | awk '{print substr($3, 2, length($3)-3)}')"
echo $revision
echo "Package: schumix" >> control
echo "Version: $revision" >> control
echo "Architecture: all" >> control
echo "Maintainer: Csaba Jakosa <megax@yeahunter.hu>" >> control
echo "Depends: mono-devel (>= 2.10.8)" >> control
echo "Section: Schumix" >> control
echo "Priority: optional" >> control
echo "Description: Schumix2 IRC Bot and Framework" >> control

cd ..
find . -exec md5sum '{}' \; > debian/md5sums
mv debian DEBIAN
cd ..
dpkg-deb --build package
echo "mv package.deb schumix.deb"
mv package.deb schumix.deb
echo "Success :)"