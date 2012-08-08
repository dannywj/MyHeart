//////第一个初始化
$(function () {
    var selectedEffect = 'explode';
    var options = {};

    if (selectedEffect === "scale") {
        options = { percent: 0 };
    } else if (selectedEffect === "size") {
        options = { to: { width: 200, height: 60} };
    }
    //淡入
    $("#effect").show('scale', options, 1000);

    //拓展消失
    setTimeout(function runEffect() {
        $("#effect").toggle(selectedEffect, options, 2000);
    }, 5000);
});

///////////第二个初始化
$(function () {
    //初始化提交表单
    // a workaround for a flaw in the demo system (http://dev.jqueryui.com/ticket/4375), ignore!
    $("#dialog:ui-dialog").dialog("destroy");
    var email = $("#email");
    var allFields = $([]).add(email);
    var tips = $("#validateTips");
    function updateTips(t) {
        tips.addClass("ui-state-highlight").text(t);
        setTimeout(function () {
            tips.removeClass("ui-state-highlight");
        }, 1000);
    }

    function checkLength(o, n, min, max) {
        if (o.val().length > max || o.val().length < min) {
            o.addClass("ui-state-error");
            updateTips(n + "长度必须在" +
					min + " 到 " + max + "个字符之间.");
            return false;
        } else {
            return true;
        }
    }

    function checkRegexp(o, regexp, n) {
        if (!(regexp.test(o.val()))) {
            o.addClass("ui-state-error");
            updateTips(n);
            return false;
        } else {
            return true;
        }
    }

    $("#dialog-form").dialog({
        autoOpen: false,
        height: 300,
        width: 350,
        modal: true,
        buttons: {
            "留下纪念": function () {
                var bValid = true;
                allFields.removeClass("ui-state-error");
                bValid = bValid && checkLength($("#email"), "email", 4, 80);
                bValid = bValid && checkRegexp(email, /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i, "请输入正确的Email格式");

                if (bValid) {
                    ///////////AJAX提交数据
                    var dayTitle = $("#txtTitle").val();
                    var content = $("#txtContent").val();
                    var customerEmail = $("#email").val();
                    $.get("control/controler.ashx", { type: 'loveday', date: new Date(), dayTitle: dayTitle, content: content, customerEmail: customerEmail }, function (data) {
                        if (data.length > 0) {
                            if (data == "success") {
                                success();
                            }
                            else {
                                error();
                            }
                        }
                    });
                    //////////
                    $(this).dialog("close");
                }
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        },
        close: function () {
            allFields.val("").removeClass("ui-state-error");
        }
    });
});

////////弹出层的提示信息
function success() {
    $("#pubsuccess").dialog();
}
function datanull() {
    $("#datanull").dialog();
}
function error() {
    $("#puberror").dialog();
}

//初始化按钮样式，时间   
$(function () {
    $("input:submit, a, button", ".demo").button();
    $("a", ".demo").click(function () { return false; });
    var dd = date2str(new Date());
    $("#txtdate").val(dd);
});
//获取当前时间 并转换
function date2str(d) {
    var ret = d.getFullYear() + "-"
    ret += ("00" + (d.getMonth() + 1)).slice(-2) + "-"
    ret += ("00" + d.getDate()).slice(-2) + " "
    ret += ("00" + d.getHours()).slice(-2) + ":"
    ret += ("00" + d.getMinutes()).slice(-2) + ":"
    ret += ("00" + d.getSeconds()).slice(-2)
    return ret;
}
//发布事件
function pub() {
    var dayTitle1 = $("#txtTitle").val();
    var content1 = $("#txtContent").val();
    if (dayTitle1 == '' || content1 == '') {
        $("#datanull").dialog();
        return false;
    }
    $("#dialog-form").dialog("open");
}

function aboutMe() {
    $("#dialog_about_us").dialog();
}
function contactMe() {
    $("#dialog_contact_us").dialog();
}
function aboutLoveDay() {
    $("#aboutLoveDay").dialog();
}
function exitLoveDay() {
    $("#dialog-confirm").dialog({
        resizable: false,
        height: 180,
        modal: true,
        buttons: {
            "确定离开": function () {
                window.location.href = "http://180.86.43.224/Login.aspx";
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });
}

