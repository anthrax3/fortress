#r "packages/fake/tools/FakeLib.dll"
#r "System.Management.Automation"

open Fake
open Fake.Testing.NUnit3
open System
open System.Management.Automation

let logo = "Fortress: "
let solution = "fortress.sln"
let projectFiles = "src/**/*.csproj"
let coreTestAssemblies = "src/Core/**/Castle.*.Tests/*.csproj"
let desktopTestAssemblies = "src/Desktop/**/Castle.*.Tests/*.csproj"
let dotNetCli = sprintf "%s\Microsoft\dotnet\dotnet.exe\r\n" (environVar "LOCALAPPDATA")

Target "Install" ignore

Target "InstallDotNetCli" <| fun _ -> 
    PowerShell.Create()
        .AddScript(". { iwr -useb https://raw.githubusercontent.com/dotnet/cli/master/scripts/obtain/dotnet-install.ps1 } | iex; install")
        .Invoke()
        |> Seq.iter (printfn "%O")
    setEnvironVar "DOTNET_CLI_TELEMETRY_OPTOUT" "1"
    printf "%sdotnet cli is now available at '%s'" logo dotNetCli

Target "InstallDotNetPackages" <| fun _ -> 
    !! projectFiles
    |> Seq.iter (fun p ->
        let result = directExec (fun info ->
                info.FileName <- dotNetCli
                info.Arguments <- (sprintf "restore %s" p))
        if result <> true then 
            failwithf "%sdotnet restore failed\r\n" logo)

Target "Build" <| fun _ ->
    tracef "%sC# Build\r\n" logo
    let setParams defaults =
            { defaults with
                Verbosity = Some(Quiet)
                Targets = ["Build"]
                Properties =
                    [
                        "Optimize", "True"
                        "DebugSymbols", "False"
                        "Configuration", "Debug"
                    ]
             }
    build setParams solution
          |> DoNothing

Target "Test" ignore

Target "TestDesktop" <| fun _ ->
    !! desktopTestAssemblies
    |> Seq.iter (fun p ->
        let result = directExec (fun info ->
                info.FileName <- dotNetCli
                info.Arguments <- (sprintf "test %s" p))
        if result <> true then 
            failwithf "%sdotnet test failed\r\n" logo)

Target "TestCore" <| fun _ -> 
    !! coreTestAssemblies
    |> Seq.iter (fun p ->
        let result = directExec (fun info ->
                info.FileName <- dotNetCli
                info.Arguments <- (sprintf "test %s" p))
        if result <> true then 
            failwithf "%sdotnet test failed\r\n" logo)

"InstallDotNetCli"
    ==> "InstallDotNetPackages"
    ==> "Install"

"Install"
    ==> "Build"
    ==> "Test"

"TestDesktop"
    ==> "TestCore"
    ==> "Test"
 
RunTargetOrDefault "Build"
