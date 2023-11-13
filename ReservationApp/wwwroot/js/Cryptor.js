export default class Cryptor {
    constructor(key = "1234567812345678", iv = "8765432187654321") {
        this.key = key;
        this.iv = iv;
    }

    async createKey(key, keyUsages) {
        const keyBuffer = new TextEncoder().encode(key);
        const cryptoKey = await window.crypto.subtle.importKey(
            "raw",
            keyBuffer,
            { name: "AES-CBC", length: 128 },
            false,
            [keyUsages]
        );
        return cryptoKey;
    }

    b64EncodeUnicode(str) {
        return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g, function (match, p1) {
            return String.fromCharCode(parseInt(p1, 16))
        }));
    }

    // Decoding base64 ⇢ UTF8

    b64DecodeUnicode(str) {
        return decodeURIComponent(Array.prototype.map.call(atob(str), function (c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)
        }).join(''));
    }
    async encrypt(inputString) {

        const cryptoKey = await this.createKey(this.key, "encrypt");

        const dataBuffer = new TextEncoder().encode(inputString);
        const ivBuffer = new TextEncoder().encode(this.iv);

        const encryptedBuffer = await window.crypto.subtle.encrypt(
            { name: "AES-CBC", iv: ivBuffer },
            cryptoKey,
            dataBuffer
        );

        return btoa(String.fromCharCode(...new Uint8Array(encryptedBuffer)));
    }

    async decrypt(encryptedString) {

        try {
            const cryptoKey = await this.createKey(this.key, "decrypt");
            const encryptedBytes = new Uint8Array(atob(encryptedString).split('').map(c => c.charCodeAt(0)));
            //const encryptedBytes = new Uint8Array(atob(encryptedString).split('').map(c => c.charCodeAt(0)));
            //const encryptedBytes = Uint8Array.from(atob(encryptedString), c => c.charCodeAt(0));
            const ivBuffer = new TextEncoder().encode(this.iv);

            const decryptedBuffer = await window.crypto.subtle.decrypt(
                { name: "AES-CBC", iv: ivBuffer },
                cryptoKey,
                encryptedBytes
            );

            return new TextDecoder().decode(decryptedBuffer);
        } catch (error) {
            alert(encryptedString + ", " + error);
            return new Promise(reject(error));
        }
        
    }
}