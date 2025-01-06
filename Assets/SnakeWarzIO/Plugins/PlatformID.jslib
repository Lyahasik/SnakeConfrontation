var DetectPlatform = {
   IsMobile: function()
   {
      return Module.SystemInfo.mobile;
   }
};  
mergeInto(LibraryManager.library, DetectPlatform);