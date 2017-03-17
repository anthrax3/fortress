#r "packages/fake/tools/FakeLib.dll"

open Fake
open Fake.Testing.NUnit3

Target "Build" <| fun _ ->
    tracef "Castle.Windsor: C# Release Build\r\n"
    let setParams defaults =
            { defaults with
                Verbosity = Some(Quiet)
                Targets = ["Build"]
                Properties =
                    [
                        "Optimize", "True"
                        "DebugSymbols", "False"
                        "Configuration", "Release"
                    ]
             }
    build setParams "Castle.Windsor.sln"
          |> DoNothing

Target "Test" <| fun _ ->
    tracef "Castle.Windsor: Running Unit Tests\r\n"
    !! ("src/Castle.Windsor.Tests/bin/debug/net461/Castle.Windsor.Tests.dll")
    |> NUnit3 (fun p ->
        {p with
            ShadowCopy = false
            ToolPath = "./packages/NUnit.ConsoleRunner/tools/nunit3-console.exe" })
    trace "##teamcity[importData type='nunit' path='.\TestResults.xml']"


RunTargetOrDefault "Build"
