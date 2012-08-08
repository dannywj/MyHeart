/// <reference path="Common.js" />
/// <reference path="AutoPlugin/jquery-1.6.2.js" />
/// <reference path="AutoPlugin/jquery-1.6.2-vsdoc.js" />

//全局函数
var ControllerPath = "../";
var CurrentUser = ''; //UserLoginName

(function ($) {
    $.extend({
        Request: function (m) {
            var url = location.href;
            var query = url.replace(/^[^\?]+\??/, '');
            var Params = {};
            if (!query) { return null; } // return null
            var Pairs = query.split(/[;&]/);
            for (var i = 0; i < Pairs.length; i++) {
                var KeyVal = Pairs[i].split('=');
                if (!KeyVal || KeyVal.length != 2) { continue; }
                var key = unescape(KeyVal[0]);
                var val = unescape(KeyVal[1]);
                val = val.replace(/\+/g, ' ');
                Params[key] = val;
            }
            return Params[m];
        }
    });
})(jQuery);

//JS拓展replace
String.prototype.replaceAll = function (s1, s2) {
    return this.replace(new RegExp(s1, "gm"), s2);
}



function RenderTemplatefunction(container, template, data) {

    $(template).tmpl(data).appendTo(container);

};
//显示愿望弹出层
function showDialog() {
    $(".heartInfo_inline").colorbox({
        inline: true, width: "50%", scrolling: false, width: "520px", height: "460px",
        onClosed: function () { $("#heartInfo_content").css({ 'display': 'none' }); }
    });
};

function FinishSignup() {
    var registerName = $("#reg_username").val();
    $("#reg_username").val('');
    $("#reg_password").val('');
    $("#reg_password_confirm").val('');
    $("#close").click();
    $("#RegisterErrorMsg").hide();
    $("#WelcomeName").text("欢迎你：" + registerName + "~");
    $("#LoginForm").hide();
    $("#UserInfo").show();
}

function GetData() {

    //获取许愿墙数据
    $.get(ControllerPath + "Heart/GetAllHeart", {}, function (data) {
        $("#content_loading").hide();//fadeOut(100);

        var jsonData = eval("(" + data + ")");
        //alert(jsonData.table[1].title);
        $("#ItemList").empty();
        RenderTemplatefunction($("#ItemList"), $("#ItemTemplate"), jsonData.table);

        $("#ItemList").children("dd").each(function (index) {

            var tTr = this;
            var selectedItem = $.tmplItem(this);

            var tmp_title = $(tTr).find("#item_title");
            var tmp_person = $(tTr).find("#item_person");
            var tmp_date = $(tTr).find("#item_date");
            var btnTitle = $(tTr).find("#btnTitle");

            var bgNumber = "it" + Math.floor(Math.random() * 10 + 9) + ".jpg"; //1-10的随机数
            var bg = $(tTr).find(".bg");
            bg.css('background-image', "url('../Content/img/bg/" + bgNumber + "')");

            var getRandomColor = function () {
                return (function (m, s, c) {
                    return (c ? arguments.callee(m, s, c - 1) : '#') +
                        s[m.floor(m.random() * 16)]
                })(Math, '0123456789abcdef', 5)
            }

            var Color = getRandomColor();
            $(tTr).find("#item_title").css('color', Color.toString());

            //绑定数据
            tmp_title.html(selectedItem.data.title);
            tmp_person.html(selectedItem.data.pubName);
            tmp_date.html(selectedItem.data.addDate.toString().split(' ')[0].replaceAll('/', '-').toString());

            btnTitle.click(function () {
                var heart_date = "";
                if (selectedItem.data.beginDate.toString() == selectedItem.data.endDate.toString()) {
                    heart_date = selectedItem.data.beginDate.toString().split(' ')[0].replaceAll('/', '-');
                }
                else {
                    heart_date = selectedItem.data.beginDate.toString().split(' ')[0].replaceAll('/', '-') + " 至 " + selectedItem.data.endDate.toString().split(' ')[0].replaceAll('/', '-');
                }
                $("#heart_title").html(selectedItem.data.title);
                $("#heart_content").html(selectedItem.data.content);
                $("#heart_date").html(heart_date);
                $("#heart_person").html(selectedItem.data.participator);
                $("#heart_contact").html(selectedItem.data.contact);
                $("#heatr_puber").html(selectedItem.data.pubName);
                //ShowBox
                this.href = "#heartInfo_content";
                $(this).addClass("heartInfo_inline");
                $("#heartInfo_content").show();
                showDialog();
            });
        });
    });
}

function InitSinaLogin() {
    $.get(ControllerPath + "User/OAuthInit", {}, function (data) {
        if (data.isSuccess != true) {
            $("#login_sina_weibo").html("<a href='" + data.url + "'>新浪微博登陆</a>");
        }
    });
}

function GetSinaUserData(code) {
    $.get(ControllerPath + "User/OAuthLogin", { code: code }, function (data) {
        if (data.isSuccess != true) {
            alert('error');
            //$("#login_sina_weibo").html("<a href='" + data.url + "'>新浪微博登陆</a>");
        }
        else {
            var userData = data.Data;
            var user = JSON.parse(userData);
            alert('欢迎你~' + user.screen_name);
        }
    });
}
//页面加载
$(function () {
    //$("#content").jqmShow();
    $("#dialog:ui-dialog").dialog("destroy");


    //获取许愿墙数据
    var sinacode = $.Request("code");

    if (!sinacode) {
        //获取新浪微博登陆链接
        InitSinaLogin();
    }
    else {
        GetSinaUserData(sinacode);
    }
    GetData();


    //check email
    $("#reg_username").blur(function () {
        $.get(ControllerPath + "User/CheckNewUser", { userName: $("#reg_username").val() }, function (data) {
            if (data.isSuccess != true) {
                $("#reg_username").val('');
                $("#reg_username").focus();
                $("#RegisterErrorMsg").text('该用户已注册，请重新输入!');
                $("#RegisterErrorMsg").show();
            }
            else {
                $("#RegisterErrorMsg").hide();
            }
        });
    });
    //注册新用户
    $("#btnRegister").click(function () {
        var userName = $("#reg_username").val();
        var userPwd = $("#reg_password").val();
        var userPwd2 = $("#reg_password_confirm").val();
        if (userName == '' || userPwd == '' || userPwd2 == '') {
            $("#RegisterErrorMsg").text('请输完整的注册信息!');
            $("#RegisterErrorMsg").show();
            return false;
        }
        if (userPwd != userPwd2) {
            $("#RegisterErrorMsg").text('两次密码不一致，请重新输入!');
            $("#reg_password").val('');
            $("#reg_password_confirm").val('');
            $("#reg_password").focus();
            $("#RegisterErrorMsg").show();
            return false;
        }

        //Register
        $.get(ControllerPath + "User/RegisterNewUser", { userName: userName, password: userPwd }, function (data) {
            if (data.isSuccess === true) {
                $("#RegisterErrorMsg").text('注册成功!');
                $("#RegisterErrorMsg").show();
                setTimeout("FinishSignup()", 2000);
                $(".pubHeart").css("display", "inline-block");
                $("#open").text("我的信息");
                CurrentUser = userName;
            }
            else {
                $("#RegisterErrorMsg").text('注册失败!');
                $("#RegisterErrorMsg").show();
            }
        });
    });
    //用户登陆
    $("#btnLogin").click(function () {
        var userName = $("#loginusername").val();
        var password = $("#loginpassword").val();
        if (userName == '' || password == '') {
            $("#LoginMessage").text('请输入用户名密码!');
            $("#LoginMessage").show();
            return false;
        }
        $("#LoginMessage").hide();
        //Login
        $.get(ControllerPath + "User/UserLogin", { userName: userName, password: password }, function (data) {
            if (data.isSuccess === true) {
                $("#LoginForm").hide();
                $("#UserInfo").show();
                $("#open").text("我的信息");
                $("#info_userLoginName").text(userName);
                //自动收起
                $("#WelcomeName").text("欢迎你：" + userName + "~");
                $(".pubHeart").css("display", "inline-block");
                setTimeout('$("#close").click();', 2000);
                CurrentUser = userName;
            }
            else {
                $("#LoginMessage").text('用户名或密码错误!');
                $("#LoginMessage").show();
                $("#loginpassword").val('');
                $("#loginusername").focus();
            }
        });


    });
    //用户退出
    $("#btnExit").click(function () {
        $("#LoginForm").show();
        $("#UserInfo").hide();
        $("#WelcomeName").text("欢迎来到许愿墙~");
        $(".pubHeart").css("display", "none");
        $("#loginusername").val('');
        $("#loginpassword").val('');
    });


    //显示发布愿望弹出层
    $(".pubHeart").colorbox({
        inline: true, width: "50%", scrolling: false, width: "480px", height: "450px",
        onOpen: function () { $(".PubNewHeart").show(); $("#hDate").val(''); },
        onClosed: function () { $(".PubNewHeart").hide(); }
    });

    //绑定日期控件
    $.datepicker.setDefaults($.datepicker.regional[""]);
    $("#hDate").datepicker($.datepicker.regional["zh-CN"]);
    //绑定发布心愿按钮事件

    $("#btnPubNewHeart").click(function () {

        var hTitle = $("#hTitle").val();
        var hPuber = $("#hPuber").val();
        var hJoiner = $("#hJoiner").val();
        var hContact = $("#hContact").val();
        var hDate = $("#hDate").val();
        var hContent = $("#hContent").val();

        var NewHeart = {
            "Title": hTitle,
            "Puber": hPuber,
            "Joiner": hJoiner,
            "Contact": hContact,
            "FinishDate": hDate,
            "HeartContent": hContent
        };

        //  alert(hTitle + hPuber + hJoiner + hContact + hDate + hContent);
        $.post(ControllerPath + "Heart/PublishNewHeart", { NewHeart: CommonJS.ToSerialize(NewHeart) }, function (data) {
            if (data.isSuccess === true) {
                // $("#close_colorbox").click(function () {
                jQuery().colorbox.close(); //不用写id也一样可以实现关闭功能
                //   });
                GetData();
            }
            else {
                //                $("#RegisterErrorMsg").text('注册失败!');
                //                $("#RegisterErrorMsg").show();
                //alert('Error!');
            }
        });

    });
});

