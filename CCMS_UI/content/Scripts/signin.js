function dec2hex(dec) {
    return ('0' + dec.toString(16)).substr(-2)
}

function generateId(len) {
    var arr = new Uint8Array((len || 40) / 2)
    var crypto = window.crypto || window.msCrypto;
    crypto.getRandomValues(arr);
    var outputString = "";
    for (var i = 0; i < arr.length; i++) {
        if (i > 0) outputString += "";
        outputString += dec2hex(arr[i]);
    }
    return outputString;
    //return Array.from(arr, dec2hex).join('')
}

function encryp(text) {
    var key = CryptoJS.enc.Utf8.parse('8080808080808080');
    var iv = CryptoJS.enc.Utf8.parse('8080808080808080');

    return CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(text), key,
    {
        keySize: 128 / 8,
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
    });
}