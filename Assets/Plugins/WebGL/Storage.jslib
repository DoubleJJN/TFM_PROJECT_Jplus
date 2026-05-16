mergeInto(LibraryManager.library, {
    SaveToLocalStorage: function(key, value) {
        key = UTF8ToString(key);
        value = UTF8ToString(value);
        localStorage.setItem(key, value);
    },

    LoadFromLocalStorage: function(key, bufferSize) {
        key = UTF8ToString(key);
        var value = localStorage.getItem(key);
        
        if (value === null) {
            value = "";
        }
        
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },

    ClearLocalStorage: function(key) {
        key = UTF8ToString(key);
        localStorage.removeItem(key);
    },

    HasLocalStorage: function(key) {
        key = UTF8ToString(key);
        return localStorage.getItem(key) !== null ? 1 : 0;
    }
});
