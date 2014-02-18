/// <reference path="Plugin/jquery-1.4.1.js" />
/// <reference path="Plugin/jquery-1.4.1-vsdoc.js" />
//全局变量
var ControllerPath = "../";

//JS拓展replace
String.prototype.replaceAll = function (s1, s2) {
    return this.replace(new RegExp(s1, "gm"), s2);
}

$(function () {
    //搜索事件
    $("#btnSearch").click(function () {
        var loadingHtml = '<div style="text-align:center"><img src="../Content/images/loadingSmall.gif"  /></div>';
        var date = $("#txtDate").val();
        if (date == '') {
            alert('请选择日期。');
            return false;
        }
        $("#lbl_msg_content").html(loadingHtml);
        $(".contentkuang").slideDown();
        $.get(ControllerPath + "Message/GetMessageByDate", { date: date }, function (data) {
            var list = data.messageList;
            var html = getMessageHTML(data.messageList);
            $("#lbl_msg_content").empty();
            $("#lbl_msg_content").html(html);
            $("#lbl_msg_title").html(date);
            if (list.length == 0) {
                $("#lbl_msg_content").html("<span style='color:#666;'>亲爱的，今天还没有悄悄话哦~ </span>");
            }
        });
    });

    //初始化日期控件
    $("#txtDate").datepicker({ minDate: '2013-10-07' });

    //获取随机消息
    function getData() {
        $.get(ControllerPath + "Message/GetRandomMessage", {}, function (data) {
            var tempData = [];
            tempData.list = data.messageList;
            tempData.title = "testTitle";
            var html = template.render('template', tempData);
            document.getElementById('container').innerHTML = html;
            //每行数据的事件绑定
            $("#AllItem").children("div").each(function (index) {//获取模板div的第一个孩子节点，注意层次关系，其间不要穿插其他元素
                var tTr = this;
                var selectedItem = tempData.list[index];//获取当前行数据对象，暂时使用数据索引方式访问。
                //绑定事件
                var aButton = $(tTr).find("#btnShowAll");
                aButton.click(function () {
                    showMessageDetail(selectedItem.PubDate);
                });
            });
        });
    }

    //初始化
    function initPage() {
        checkLogin();
        getData();
    }

    //显示消息详情
    function showMessageDetail(date) {
        $("#txtDate").val(date);
        $("#btnSearch").click();
    }

    //生成html
    function getMessageHTML(json) {
        var html = '';
        for (var i = 0; i < json.length; i++) {
            json[i].Writer
            json[i].Content
            if (json[i].Writer == 'juejue') {
                html += '<p class="meTxt">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + (json[i].Content) + '</p>';
            }
            if (json[i].Writer == 'gege') {
                html += '<p class="gegeTxt">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + (json[i].Content) + '</p>';
            }
        }
        return html;
    }

    //验证身份
    function checkLogin() {
        $('.hide_bg').fadeIn(1000);
        $('.hide_content').fadeIn(400);
        $("#txtPwd").keyup(function () {
            var pwd = hex_md5($("#txtPwd").val());
            if (pwd === '0768281a05da9f27df178b5c39a51263') {
                $('.hide_bg').fadeOut(400);
                $('.hide_content').fadeOut(400);
            }
        });
    }

    initPage();

});