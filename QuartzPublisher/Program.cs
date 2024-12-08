﻿using System;
using System.CommandLine;
using Microsoft.Extensions.Configuration;

namespace QuartzPublisher
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var config = ConfigurationProvider.GetConfiguration();
            
            var sourceDirectoryOption = new Option<DirectoryInfo?>(
                name: "--source",
                description: "Source directory for files to publish, e.g. /path/to/vault",
                getDefaultValue: () => ConfigurationProvider.GetDirectory(config["source"])
            );
            sourceDirectoryOption.AddAlias("-s");
            
            var destinationDirectoryOption = new Option<DirectoryInfo?>(
                name: "--destination",
                description: "Destination directory for files to publish, e.g. /path/to/content",
                getDefaultValue: () => ConfigurationProvider.GetDirectory(config["destination"])
            );
            destinationDirectoryOption.AddAlias("-d");
            
            var verboseOption = new Option<bool>(
                name: "--verbose",
                description: "Enable verbose output",
                getDefaultValue: () => false
            );
            verboseOption.AddAlias("-v");
            
            var noDeleteOption = new Option<bool>(
                name: "--no-delete",
                description: "Disable deletion of files not marked for publishing",
                getDefaultValue: () => ConfigurationProvider.GetBool(config["noDelete"], defaultValue: false)
            );
            
            
            var rootCommand = new RootCommand("Publishes files from a source directory to a target directory")
            {
                sourceDirectoryOption,
                destinationDirectoryOption,
                verboseOption,
                noDeleteOption
            };
            
            rootCommand.SetHandler((DirectoryInfo? source, DirectoryInfo? destination, bool verbose, bool noDelete) =>
            {
                if (source is null || destination is null)
                {
                    Console.WriteLine("Source and destination directories must be provided.");
                    return;
                }
                
                Publisher.PublishContent(source?.FullName!, destination?.FullName!, verbose, noDelete);
            }, sourceDirectoryOption, destinationDirectoryOption, verboseOption, noDeleteOption);

            return rootCommand.Invoke(args);
        }

        
    }
}