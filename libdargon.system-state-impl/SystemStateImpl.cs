using System;
using System.IO;
using ItzWarty;
using ItzWarty.IO;

namespace Dargon {
   public class SystemStateImpl : SystemState {
      private const string kSystemStateConfigurationDirectoryName = "system-state";
      private readonly IFileSystemProxy fileSystemProxy;
      private readonly IDargonConfiguration configuration;
      private readonly string basePath;

      public SystemStateImpl(IFileSystemProxy fileSystemProxy, IDargonConfiguration configuration) {
         this.fileSystemProxy = fileSystemProxy;
         this.configuration = configuration;
         this.basePath = Path.Combine(configuration.ConfigurationDirectoryPath, kSystemStateConfigurationDirectoryName);
         fileSystemProxy.PrepareDirectory(basePath);
      }

      public string Get(string key, string defaultValue) {
         var path = BuildKeyPath(key);
         var fileInfo = fileSystemProxy.GetFileSystemInfo(path);
         if (fileInfo.Exists && !fileInfo.Attributes.HasFlag(FileAttributes.Directory)) {
            return fileSystemProxy.ReadAllText(path);
         } else {
            return defaultValue;
         }
      }

      public void Set(string key, string value) {
         var path = BuildKeyPath(key);
         fileSystemProxy.PrepareParentDirectory(path);
         fileSystemProxy.WriteAllText(path, value);
      }

      private string BuildKeyPath(string key) {
         var parts = key.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
         var path = basePath;
         foreach (var part in parts) {
            path = Path.Combine(path, part);
         }
         return path;
      }
   }
}