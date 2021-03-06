﻿using NLog;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Dargon.Processes.Injection {
   public class SafeRemoteBufferHandle : SafeHandleZeroIsInvalid {
      private readonly static Logger logger = LogManager.GetCurrentClassLogger();

      private readonly IntPtr hProcess;

      public SafeRemoteBufferHandle(
         IntPtr hProcess,
         IntPtr pRemoteBuffer, 
         bool ownsHandle = true
      ) : base(pRemoteBuffer, ownsHandle) {
         this.hProcess = hProcess;
      }

      protected override bool ReleaseValidHandleInternal(IntPtr pRemoteBufferCapture) {
         return WinAPI.VirtualFreeEx(
            hProcess,
            pRemoteBufferCapture,
            0,
            FreeType.Release
         );
      }

      public static SafeRemoteBufferHandle AllocateOrThrow(SafeProcessHandle hProcessSafe, string text) {
         var hProcessUnsafe = hProcessSafe.DangerousGetHandle();

         logger.Info($"Allocating remote string buffer for text {text}.");
         uint szText = (uint)(text.Length + 1); // +1 for null string terminator

         IntPtr pRemoteStringBuffer = WinAPI.VirtualAllocEx(
            hProcessUnsafe,
            IntPtr.Zero,
            szText,
            AllocationType.Commit,
            MemoryProtection.ExecuteReadWrite
         );

         if (pRemoteStringBuffer == IntPtr.Zero) {
            throw new Exception(
               "Could not allocate memory inside target process!\r\n" +
               "\r\nDll Path: " + text + " (size " + szText + ")" +
               "\r\nErrno: " + Marshal.GetLastWin32Error()
            );
         }

         logger.Info("Write string in remote process");
         int bytesWritten;
         bool writeSuccessful = WinAPI.WriteProcessMemory(
            hProcessUnsafe,
            pRemoteStringBuffer,
            Encoding.ASCII.GetBytes(text),
            szText,
            out bytesWritten
         );
         if (!writeSuccessful) {
            throw new Exception(
               "Call to WriteProcessMemory failed! \n" +
               "\nDll Path: " + text + " (size " + szText + ")" +
               "\nBytes Written: " + bytesWritten +
               "\nErrno: " + Marshal.GetLastWin32Error()
            );
         } else if (bytesWritten != szText) {
            throw new Exception(
               "WriteProcessMemory did not write expected byte count! \n" +
               "\nDll Path: " + text + " (size " + szText + ")" +
               "\nBytes Written: " + bytesWritten +
               "\nErrno: " + Marshal.GetLastWin32Error()
            );
         }

         return new SafeRemoteBufferHandle(hProcessUnsafe, pRemoteStringBuffer);
      }
   }
}
