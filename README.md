# GiraffeApp

A [Giraffe](https://github.com/giraffe-fsharp/Giraffe) web application, which has been created via the `dotnet new giraffe` command.

## Build and test the application

### Windows

Run the `build.bat` script in order to restore, build and test (if you've selected to include tests) the application:

```
> ./build.bat
```

### Linux/macOS

Run the `build.sh` script in order to restore, build and test (if you've selected to include tests) the application:

```
$ ./build.sh
```

## Run the application

After a successful build you can start the web application by executing the following command in your terminal:

```
dotnet run src/GiraffeApp
```

After the application has started visit [http://localhost:5000](http://localhost:5000) in your preferred browser.

## Extreme-startup

[extreme_startup on github](https://github.com/rchatley/extreme_startup)

When running Extreme_startup from docker and your client server is running on the host machine, then use this hostname to registre on the server *docker.for.win.localhost* in order for the extreme_startup server to connect to your client server.

This fsharp client server can be reached from inside the docker container on this url: http://docker.for.win.localhost:5000

**Helpfull Docker commands**

Start server in warmup mode:
```
docker run --name es -p 3000:3000 -e WARMUP=1 extreme_startup
```

Start server in battle mode:
```
docker run --name es -p 3000:3000 extreme_startup
```

Attach to runnig server:
```
docker attach es
```

## Links

**Extreme_startup server**

[Server main page](http://localhost:3000/)  
[Server control panel](http://localhost:3000/controlpanel)  

**F# client server**

[]