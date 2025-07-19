// ABP FreezeUI integration
(function() {
  if (typeof abp === 'undefined') {
    window.abp = {};
  }
  
  abp.ui = {
    freeze: function() {
      if (typeof FreezeUI !== 'undefined') {
        FreezeUI();
      }
    },
    unfreeze: function() {
      if (typeof UnFreezeUI !== 'undefined') {
        UnFreezeUI();
      }
    }
  };
})();