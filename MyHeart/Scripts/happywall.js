﻿/// <reference path="Common.js" />
/// <reference path="AutoPlugin/jquery-1.6.2.js" />
/// <reference path="AutoPlugin/jquery-1.6.2-vsdoc.js" />

//全局变量
var ControllerPath = "../";
var gCurrentUser = ''; //UserLoginName
var gUserNickName = '';
var gHeartLevel = 1;
var gMaxNum = 18;//max 比实际图片数小1
var gRandomNumList = [];
var gPubHeartJoinerLoginName = '';
var gMsgSetIntervalId = 0;

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

String.prototype.Trim = function () {
    return this.replace(/(^\s*)|(\s*$)/g, "");
}

function RenderTemplatefunction(container, template, data) {

    $(template).tmpl(data).appendTo(container);

};

jQuery.fn.clickOnce = function (callback) {
    this.unbind('click');
    this.click(function () {
        callback();
    });
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
    $("#WelcomeName").text("欢迎你：" + gUserNickName + "~");
    $("#info_userLoginName").text(gUserNickName + '[' + gCurrentUser + ']');
    $("#LoginForm").hide();
    $("#UserInfo").show();
}

//检查生成的随机数是否之前出现过
function checkRandomNum(num) {
    if ($.inArray(num, gRandomNumList) == -1) {
        gRandomNumList.push(num);
        return true;
    } else {
        if (gRandomNumList.length == gMaxNum) {//所有的随机数都出现过，则允许重复
            return true;
        }
        return false;
    }
}

function GetData() {

    //获取许愿墙数据
    $.get(ControllerPath + "Heart/GetAllHeart?date=" + new Date(), { date: new Date() }, function (data) {
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

            //随机生成背景文字配色方案

            var randomNum = CommonJS.RandomBy(1, gMaxNum);
            while (!checkRandomNum(randomNum)) {
                randomNum = CommonJS.RandomBy(1, gMaxNum);
            }

            var bg = $(tTr).find(".bg");
            bg.css('background-image', "url('../Content/img/bg/" + HeartBGConfig[randomNum].BGImage + "')");
            $(tTr).find("#item_title").css('color', HeartBGConfig[randomNum].TitleColor);
            //随机生成背景文字配色方案End

            //var bgNumber = "it" + Math.floor(Math.random() * 10+11 ) + ".jpg"; //1-10的随机数
            //var getRandomColor = function () {
            //    return (function (m, s, c) {
            //        return (c ? arguments.callee(m, s, c - 1) : '#') +
            //            s[m.floor(m.random() * 16)]
            //    })(Math, '0123456789abcdef', 5)
            //}

            //var Color = getRandomColor();


            //绑定数据
            tmp_title.html(selectedItem.data.title);
            tmp_person.html(selectedItem.data.pubName);
            var temp_date_str = selectedItem.data.addDate.toString().split(' ')[0].replaceAll('/', '-').toString();
            if (selectedItem.data.station === '1') {
                temp_date_str += '&nbsp;<img src="../Content/img/ht_finish.png" style="height:14px;vertical-align: middle;line-height:14px;" title="已实现" />';
            }
            tmp_date.html(temp_date_str);

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
                if (selectedItem.data.endDate === '') {
                    $("#heart_date").parent().hide();
                } else {
                    $("#heart_date").html(heart_date);
                    $("#heart_date").parent().show();
                }
                if (selectedItem.data.participator === '') {
                    $("#heart_person").parent().hide();
                } else {
                    $("#heart_person").html(selectedItem.data.participator);
                    $("#heart_person").parent().show();
                }

                if (selectedItem.data.contact === '') {
                    $("#heart_contact").parent().hide();
                } else {
                    $("#heart_contact").html(selectedItem.data.contact);
                    $("#heart_contact").parent().show();
                }

                $("#heatr_puber").html(selectedItem.data.pubName);
                //ShowBox
                this.href = "#heartInfo_content";
                $(this).addClass("heartInfo_inline");
                $("#heartInfo_content").show();
                showDialog();
            });
        });
    });

    //获取用户登录状态
    $.get(ControllerPath + "User/GetUserLoginStatus?date=" + new Date(), {}, function (data) {
        //var jsonData = eval("(" + data + ")");
        if (data.isSuccess) {
            gCurrentUser = data.CurrentUser;
            gUserNickName = data.NickName;
            showLoginInfo(data.CurrentUser);
            GetHeartsCount(gCurrentUser);
            //获取消息盒子数据
            GetMessageDataCount();
            gMsgSetIntervalId = window.setInterval(GetMessageDataCount, 3000);
        }
    });

    //填充默认用户名密码
    var cookie_loginname = $.cookie('loginname');
    var cookie_password = $.cookie('password');

    if (cookie_loginname != '') {
        $("#loginusername").val(cookie_loginname);
        $("#loginpassword").val(cookie_password);
    }



}
//sina weibo login
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

//登录成功后的显示
function showLoginInfo(userName) {
    gCurrentUser = userName;
    $("#LoginForm").hide();
    $("#UserInfo").show();
    $("#open").text("我的信息");
    $("#info_userLoginName").text(gUserNickName + ' [' + gCurrentUser + ']');
    //自动收起
    $("#WelcomeName").text("欢迎你：" + gUserNickName + "~");
    $(".pubHeart").css("display", "inline-block");
    //setTimeout('$("#close").click();', 2000);
    $("#P_UserHeartInfo").show();
    $("#P_UserRegisterInfo").hide();
}

//获取个人心愿列表
function GetHeartListByLoginName(loginName) {
    $.get(ControllerPath + "Heart/GetHeartsByLoginName?date=" + new Date(), { loginName: loginName }, function (data) {
        if (data.isSuccess) {
            var html = GetHeartListHTML(data.heartList);
            $("#pMyHeartList").empty();
            $("#pMyHeartList").html(html);
        }
    });
}

function GetHeartListHTML(json) {
    var html = '';
    html += '<table cellpadding="0" cellspacing="0" border="0">';
    html += ' <tr><td class="hItem_b">心愿等级</td><td class="hItem_b hItemContent">心愿名称</td>';
    html += '<td class="hItem_b">实现日期</td><td class="hItem_b">心愿状态</td></tr>';
    for (var i = 0; i < json.length; i++) {
        html += '<tr>';
        html += '<td class="hItem">';
        for (var j = 0; j < json[i].HeartLevel; j++) {
            html += '<img class="ht_level" src="../Scripts/Plugin/ratyStar/img/star-on.png" />';
        }
        html += '</td>';
        html += '<td class="hItem hItemContent"><a title="' + json[i].HeartContent + '" href="javascript:void(0)">' + json[i].Title + '</a></td>';
        html += '<td class="hItem">' + json[i].FinishDate + '</td>';
        if (json[i].Station === 1) {//已实现
            html += '<td class="hItem"><a class="ht_a_done" href="javascript:void(0)" title="已实现" id="tests"><img src="../Content/img/heart_ok.png" /></a></td>';
        } else {
            html += ('<td class="hItem"><a onclick="ChangeHeartStationOK(\'' + json[i].HeartId + '\')" class="ht_a_done" href="javascript:void(0)" title="未实现，点击标记为实现" id="tests"><img src="../Content/img/heart_do.png" /></a></td>');
        }
        html += '</tr>';
    }
    html += '</table>';

    return html;
}

//更新心愿状态（已实现）
function ChangeHeartStationOK(id) {
    $.get(ControllerPath + "Heart/UpdateHeartStation?date=" + new Date(), { station: 1, heartId: id }, function (data) {
        if (data.isSuccess) {
            alert('真好，又实现了一个愿望！加油！！^_^');
            GetHeartListByLoginName(gCurrentUser);
            GetHeartsCount(gCurrentUser);
            GetData();
        }
    });
}

function GetHeartsCount(userName) {
    $.get(ControllerPath + "Heart/GetHeartsCount?date=" + new Date(), { loginName: userName }, function (data) {
        if (data.isSuccess) {
            $("#txtAllCount").html(data.allcount);
            $("#txtOKCount").html(data.okcount);
        }
    });
}

function isEmail(str) {
    var reg = /^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$/;
    return reg.test(str);
}

//消息盒子部分Begin
function GetMessageDataCount() {
    $.get(ControllerPath + "Heart/HasNewMessage?date=" + new Date(), { loginName: gCurrentUser }, function (data) {
        if (data.isSuccess) {
            if (data.hasNewMessage) {
                ShowMessageBox();
            } else {
                HideMessageBox();
            }
        }
    });
}

function GetMessageData() {
    $.get(ControllerPath + "Heart/GetNewMessage?date=" + new Date(), { loginName: gCurrentUser }, function (data) {
        if (data.isSuccess) {
            if (data.heartInfo.length > 0) {
                var cHTML = '';
                for (var i = 0; i < data.heartInfo.length; i++) {
                    cHTML += '<p> <div id="" class="newMsgTitle">';
                    cHTML += '<span>•</span>&nbsp;<span class="font-org">' + data.heartInfo[i].Puber + '</span>期待与你完成心愿《<span class="font-org">' + data.heartInfo[i].Title + '</span>》';
                    cHTML += '<a href="javascript:void(0);" onclick="ToggleNewMessage(\'' + data.heartInfo[i].HeartId + '\')" >心愿详情</a>';
                    cHTML += '</div><div id="' + data.heartInfo[i].HeartId + '" class="newMsgContent">';
                    cHTML += data.heartInfo[i].HeartContent;
                    cHTML += '</div> </p>';
                }
                $("#newMessageContent").html(cHTML);
                UpdateNewMessageStatus();
            } else {

            }
        }
    });
}

function UpdateNewMessageStatus() {
    $.get(ControllerPath + "Heart/UpdateNewMessageStatus?date=" + new Date(), { loginName: gCurrentUser }, function (data) {

    });
}

function ToggleNewMessage(id) {
    $("#" + id).toggle();
}

function ShowMessageBox() {
    $("#MsgBox").html('您有新动态，<a href="#P_NewMessages" class="btnNewMessage">点击查看</a>');
    //初始化消息盒子panel
    $(".btnNewMessage").colorbox({
        inline: true, width: "50%", scrolling: false, width: "480px", height: "220px",
        onOpen: function () { $(".P_NewMessages").show(); GetMessageData(gCurrentUser); },
        onClosed: function () { $(".P_NewMessages").hide(); }
    });
    $("#MsgBox").show();
}

function HideMessageBox() {
    $("#MsgBox").hide();
}
//消息盒子部分End

//用户信息更新begin

//用户信息更新end

/*
   ***********************************************
   * 页面加载，初始化
   ***********************************************
   */
//页面加载
$(function () {
    //$("#content").jqmShow();
    $("#dialog:ui-dialog").dialog("destroy");

    //初始化心愿等级
    $("#result").hide();//将结果DIV隐藏
    $('#star').raty({
        hints: ['小心愿', '惊喜', '大大的梦想', '长久的执著', '一生的梦想'],
        path: "../Scripts/Plugin/ratyStar/img",
        starOff: 'star-off-big.png',
        starOn: 'star-on-big.png',
        size: 30,
        score: 1,
        //target: '#result',
        targetKeep: true,
        click: function (score, evt) {
            gHeartLevel = score;
            //alert('u selected '+score);
        }
    });

    //var sinacode = $.Request("code");

    //if (!sinacode) {
    //    //获取新浪微博登陆链接
    //    InitSinaLogin();
    //}
    //else {
    //    GetSinaUserData(sinacode);
    //}

    //获取许愿墙数据
    GetData();

    /*
   ***********************************************
   * 注册登录部分
   ***********************************************
   */
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
        var userNickName = $("#reg_nickname").val();
        if (userName == '' || userPwd == '' || userNickName == '') {
            $("#RegisterErrorMsg").text('请输入完整的注册信息!');
            $("#RegisterErrorMsg").show();
            return false;
        }
        if (!isEmail(userName)) {
            $("#RegisterErrorMsg").text('邮箱格式不正确!');
            $("#RegisterErrorMsg").show();
            return false;
        }
        //if (userPwd != userPwd2) {
        //    $("#RegisterErrorMsg").text('两次密码不一致，请重新输入!');
        //    $("#reg_password").val('');
        //    $("#reg_password_confirm").val('');
        //    $("#reg_password").focus();
        //    $("#RegisterErrorMsg").show();
        //    return false;
        //}
        $("#RegisterErrorMsg").text('注册中，请稍候...');
        $("#RegisterErrorMsg").show();

        //Register
        $.get(ControllerPath + "User/RegisterNewUser", { userName: userName, password: hex_md5(userPwd), userNickName: userNickName }, function (data) {
            if (data.isSuccess === true) {
                gUserNickName = userNickName;
                gCurrentUser = userName;
                $("#RegisterErrorMsg").text('注册成功! 请登录。');
                $("#RegisterErrorMsg").show();
                $("#reg_username").val('');
                $("#reg_password").val('');
                $("#reg_nickname").val('');
                //setTimeout("FinishSignup()", 2000);
                //$(".pubHeart").css("display", "inline-block");
                //$("#open").text("我的信息");
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
        var md5_pwd = password;
        if (password.length < 16) {
            md5_pwd = hex_md5(password);
        }
        $.get(ControllerPath + "User/UserLogin", { userName: userName, password: md5_pwd }, function (data) {
            if (data.isSuccess === true) {
                gUserNickName = data.nickName;
                showLoginInfo(userName);
                GetHeartsCount(userName);
                //设置记住用户cookie
                if ($("#rememberMe").attr("checked") == "checked") {
                    $.cookie('loginname', userName, { expires: 7 });
                    $.cookie('password', md5_pwd, { expires: 7 });
                } else {
                    $.cookie('loginname', null);
                    $.cookie('password', null);
                }
                GetData();
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
        //获取用户登录状态
        $.get(ControllerPath + "User/UserLogout", {}, function (data) {
            if (data.isSuccess) {
                $("#LoginForm").show();
                $("#UserInfo").hide();
                $("#P_UserHeartInfo").hide();
                $("#P_UserRegisterInfo").show();
                $("#WelcomeName").text("欢迎来到许愿墙~");
                $(".pubHeart").css("display", "none");
                //$("#loginusername").val('');
                //$("#loginpassword").val('');

                GetData();
                window.clearInterval(gMsgSetIntervalId);
                HideMessageBox();
            }
        });
    });

    /*
   ***********************************************
   * 心愿发布部分
   ***********************************************
   */
    //显示发布愿望弹出层
    $(".pubHeart").colorbox({
        inline: true, width: "50%", scrolling: false, width: "480px", height: "480px",
        onOpen: function () {
            $(".PubNewHeart").show();
            $("#hTitle").val('');
            $("#hJoiner").val('');
            $("#hContact").val('');
            $("#hContent").val('');
            $("#hDate").val('');
            $("#hPuber").val(gUserNickName);
        },
        onClosed: function () { $(".PubNewHeart").hide(); }
    });

    //绑定日期控件
    //$.datepicker.setDefaults($.datepicker.regional[""]);//default english
    //$("#hDate").datepicker($.datepicker.regional["zh-CN"]);
    $("#hDate").datepicker({ minDate: 0 });

    //绑定发布心愿按钮事件
    $("#btnPubNewHeart").click(function () {
        var hTitle = $("#hTitle").val();
        var hPuber = $("#hPuber").val();
        var hJoiner = $("#hJoiner").val();
        var hContact = $("#hContact").val();
        var hDate = $("#hDate").val();
        var hContent = $("#hContent").val();
        var hIsPrivate = $("#hPublic").attr("checked") ? 0 : 1;

        //验证输入
        var errMsg = '';
        if (hTitle == '') {
            errMsg += '心愿名称';
        }
        if (hContent == '') {
            errMsg += ' 心愿内容';
        }
        if (errMsg == '') {
            $("#pubErrorMessage").hide();
        } else {
            $("#pubErrorMessage").html('请输入' + errMsg);
            $("#pubErrorMessage").show();
            return false;
        }

        var NewHeart = {
            "Title": hTitle,
            "Puber": hPuber,
            "PubId": gCurrentUser,
            "Joiner": hJoiner,
            "Contact": hContact,
            "FinishDate": hDate,
            "HeartContent": hContent,
            "HeartLevel": gHeartLevel,
            "IsPrivate": hIsPrivate
        };
        $("#spPubNote").show();
        $.post(ControllerPath + "Heart/PublishNewHeart", { NewHeart: CommonJS.ToSerialize(NewHeart), joinerLoginName: gPubHeartJoinerLoginName }, function (data) {
            if (data.isSuccess === true) {
                $("#spPubNote").hide();
                jQuery().colorbox.close(); //不用写id也一样可以实现关闭功能
                //$("#close").click();
                GetData();
            }
            else {
                $("#spPubNote").hide();
            }
        });

    });

    /*
    ***********************************************
    * 心愿管理部分
    ***********************************************
    */
    //显示管理愿望弹出层
    $(".btnManageHeart").colorbox({
        inline: true, width: "50%", scrolling: false, width: "680px", height: "550px",
        onOpen: function () { $(".P_ManageHeart").show(); $("#close").click(); GetHeartListByLoginName(gCurrentUser); },
        onClosed: function () { $(".P_ManageHeart").hide(); }
    });

    //====自动完成Begin=====
    $("#hJoiner").autocomplete({
        minLength: 0,
        source: function (request, response) {
            var term = request.term;
            $.ajax({
                url: ControllerPath + "User/GetAllUser?date=" + new Date(),
                dataType: "json",
                data: {
                    key: request.term
                },
                success: function (data) {
                    response($.map(data.userList, function (item) {
                        return {
                            //自定义返回项目
                            label: item.NickName,
                            value: item.LoginName
                        }
                    }));
                }
            });
        }
            ,
        focus: function (event, ui) {
            $("#hJoiner").val(ui.item.label);
            return false;
        },
        select: function (event, ui) {
            $("#hJoiner").val(ui.item.label);
            $("#hJoinerValue").val(ui.item.value);
            gPubHeartJoinerLoginName = ui.item.value;
            //$("#project-description").html(ui.item.desc);
            //$("#project-icon").attr("src", "images/" + ui.item.icon);
            return false;
        }
    })
        .data("autocomplete")._renderItem = function (ul, item) {
            return $("<li>")
                .data("item.autocomplete", item)
                .append("<a>" + item.label + "</a>")
                .appendTo(ul);
        };
    //====自动完成End=====

    /*
   ***********************************************
   * 用户信息修改
   ***********************************************
   */
    $(".btnUpdateUserInfo").colorbox({
        inline: true, scrolling: false, width: "400px", height: "220px",
        onOpen: function () { $(".P_UserInfoUpdate").show(); $("#updateErrorMessage").hide() },
        onClosed: function () { $(".P_UserInfoUpdate").hide(); }
    });

    $("#btnUpdateUserInfo").clickOnce(function () {
        var new_user_nickname = $("#txt_user_new_nick_name").val();
        if (new_user_nickname.Trim() == '') {
            $("#updateErrorMessage").html('请输入昵称');
            $("#updateErrorMessage").show();
            return false;
        }

        $.get(ControllerPath + "User/UpdateUserInfo", { loginName: gCurrentUser, userNickName: new_user_nickname }, function (data) {
            if (data.isSuccess === true) {
                window.location = window.location;
            }
            else {
                $("#updateErrorMessage").html('更新失败');
                $("#updateErrorMessage").show();
            }
        });



    });
});

