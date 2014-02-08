/// <reference path="Plugin/jquery-1.4.1.js" />
/// <reference path="Plugin/jquery-1.4.1-vsdoc.js" />
//全局变量
var ControllerPath = "../";

//JS拓展replace
String.prototype.replaceAll = function (s1, s2) {
    return this.replace(new RegExp(s1, "gm"), s2);
}

$(function () {
    $("#btnSearch").click(function () {
        $(".contentkuang").toggle();
    });

    //初始化日期控件
    $("#txtDate").datepicker({ minDate: '2014-01-05' });

    function getData() {
        $.get(ControllerPath + "Message/GetRandomMessage", {}, function (data) {
            var tempData = [];
            tempData.list = data.messageList;
            tempData.title = "testTitle";
            var html = template.render('template', tempData);
            document.getElementById('container').innerHTML = html;
        });
    }

    function initPage() {
        getData();
    }

    function searchMessage() {

    }

    initPage();
});