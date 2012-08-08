function CommonJS() { }

CommonJS.ToSerialize = function (obj) {
    var ransferCharForJavascript = function (s) {
        var newStr = s.replace(
        /[\x26\x27\x3C\x3E\x0D\x0A\x22\x2C\x5C\x00]/g,
        function (c) {
            ascii = c.charCodeAt(0)
            return '\\u00' + (ascii < 16 ? '0' + ascii.toString(16) : ascii.toString(16))
        }
         );
        return newStr;
    }
    if (obj == null) {
        return null
    }
    else if (obj.constructor == Array) {
        var builder = [];
        builder.push("[");
        for (var index in obj) {
            if (typeof obj[index] == "function") continue;
            if (index > 0) builder.push(",");
            builder.push(CommonJS.ToSerialize(obj[index]));
        }
        builder.push("]");
        return builder.join("");
    }
    else if (obj.constructor == Object) {
        var builder = [];
        builder.push("{");
        var index = 0;
        for (var key in obj) {
            if (typeof obj[key] == "function") continue;
            if (index > 0) builder.push(",");
            builder.push(CommonJS.FormatString("\"{0}\":{1}", key, CommonJS.ToSerialize(obj[key])));
            index++;
        }
        builder.push("}");
        return builder.join("");
    }
    else if (obj.constructor == Boolean) {
        return obj.toString();
    }
    else if (obj.constructor == Number) {
        return obj.toString();
    }
    else if (obj.constructor == String) {
        return CommonJS.FormatString('"{0}"', ransferCharForJavascript(obj));
    }
    else if (obj.constructor == Date) {
        return "'" + CommonJS.FormatString('{"__DataType":"Date","__thisue":{0}}', obj.getTime() - (new Date(1970, 0, 1, 0, 0, 0)).getTime()) + "'"; //CommonJS.FormatString('{"__DataType":"Date","__thisue":{0}}', obj.getTime() - (new Date(1970, 0, 1, 0, 0, 0)).getTime());
    }
    else if (this.toString != undefined) {
        return CommonJS.ToSerialize(obj);
    }
}

CommonJS.FormatString = function (str) {
    var i = 1, args = arguments;
    var str = args[0];
    var re = /\{(\d+)\}/g;
    return str.replace(re, function () { return args[i++] });
}
