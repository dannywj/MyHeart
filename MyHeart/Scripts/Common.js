﻿function CommonJS() { }

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

//写cookies
CommonJS.SetCookie = function (name, value) {
    var Days = 1;
    var exp = new Date();
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
}

//读取cookies
CommonJS.GetCookie = function (name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
    if (arr = document.cookie.match(reg))
        return unescape(arr[2]);
    else
        return null;
}
//删除cookies
CommonJS.DelCookie = function (name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
}

/*
通过下面方法使用：
$(document).ready(function () {
    $('#write_cokies').click(function () {
        $.cookie('name', 'test', { expires: 7 });
    });
    $('#read_ookies').click(function () {
        var test = $.cookie('name');
        alert(test);
    });
    $('#delete_cookies').click(function () {
        $.cookie('name', null);
    });
});
*/
jQuery.cookie = function (name, value, options) {
    if (typeof value != 'undefined') {
        options = options || {};
        if (value === null) {
            value = '';
            options = $.extend({}, options);
            options.expires = -1;
        }
        var expires = '';
        if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
            var date;
            if (typeof options.expires == 'number') {
                date = new Date();
                date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
            } else {
                date = options.expires;
            }
            expires = '; expires=' + date.toUTCString();
        }
        var path = options.path ? '; path=' + (options.path) : '';
        var domain = options.domain ? '; domain=' + (options.domain) : '';
        var secure = options.secure ? '; secure' : '';
        document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
    } else {
        var cookieValue = null;
        if (document.cookie && document.cookie != '') {
            var cookies = document.cookie.split(';');
            for (var i = 0; i < cookies.length; i++) {
                var cookie = jQuery.trim(cookies[i]);
                if (cookie.substring(0, name.length + 1) == (name + '=')) {
                    cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                    break;
                }
            }
        }
        return cookieValue;
    }
};