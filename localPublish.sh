rm ./bin/Debug/*.nupkg
dotnet pack
file=$(ls ./bin/Debug/*.nupkg | grep .nupkg)
echo Getting ready to publish $file

rm ~/nugetFeed/*
cp $file ~/nugetFeed/

