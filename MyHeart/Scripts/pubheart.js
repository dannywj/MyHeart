
var TextEditPanel = new Ext.form.HtmlEditor({
    fieldLabel: '心愿内容',
    enableAlignments: true,
    enableColors: true,
    enableFont: true,
    enableLists: true,
    enableSourceEdit: true,
    width: 474,
    height: 200,
    maxLength: 200,
    name: "content"
});
var MainPanel = new Ext.form.FormPanel({
    title: "我的心愿",
    labelWidth: 100,
    labelAlign: "left",
    layout: "form",
    width: 600,
    height: 500,
    padding: "10px",
    items: [
		{
		    xtype: "textfield",
		    fieldLabel: "心愿名称",
		    maxLength: 14,
		    anchor: "100%",
		    allowBlank: false,
		    name: "title"
		},
		{
		    xtype: "textfield",
		    fieldLabel: "心愿发布人",
		    anchor: "100%",
		    //inputType: "password",
		    maxLength: 7,
		    allowBlank: false,
		    name: "writer"
		},
		{
		    xtype: "textfield",
		    fieldLabel: "期待参与者",
		    anchor: "100%",
		    //inputType: "password",
		    //minLength: 6,
		    allowBlank: true,
		    name: "joiner"
		},
		{
		    xtype: "textfield",
		    fieldLabel: "联系方式(QQ,Email,Phone)",
		    anchor: "100%",
		    //vtype: "email",
		    allowBlank: true,
		    name: "contact"
		},
    ///////////////////
    //        {xtype: 'textfield',
    //            fieldLabel: 'File Name',
    //            name: 'userfile',
    //            inputType: 'file',
    //            allowBlank: false,
    //            blankText: 'File can\'t not empty.',
    //            anchor: '90%'  // anchor width by percentage
    //},
    //////////////////
    //        {
    //        xtype: "textfield",
    //        fieldLabel: "背景图片",
    //        anchor: "100%",
    //        //vtype: "email",
    //        allowBlank: true,
    //        name: "bgimage"
    //    },
		{
		xtype: "datefield",
		fieldLabel: "实现日期",
		anchor: "50%",
		allowBlank: false,
		name: "beginDate"
},
        {
            xtype: "datefield",
            fieldLabel: "",
            anchor: "50%",
            allowBlank: false,
            name: "endDate"
        },
         TextEditPanel,
		{
		    xtype: "container",
		    autoEl: "div",
		    height: 30,
		    width: 436
		},
		{
		    xtype: "container",
		    autoEl: "div",
		    height: 34,
		    layout: "hbox",
		    width: 422,
		    items: [
				{
				    xtype: "container",
				    autoEl: "div",
				    width: 107,
				    height: 23
				},
				{
				    xtype: "container",
				    autoEl: "div",
				    layout: "table",
				    width: 90,
				    height: 30,
				    items: [
						{
						    xtype: "button",
						    text: "发布心愿",
						    width: 80,
						    handler: AjaxSubmit
						}
					]
				},
				{
				    xtype: "container",
				    autoEl: "div",
				    layout: "table",
				    width: 90,
				    height: 14,
				    items: [
						{
						    xtype: "button",
						    text: "重写心愿",
						    width: 80,
						    handler: resetform
						}
					]
				}
		    //                ,
		    //                {
		    //                    xtype: "container",
		    //                    autoEl: "div",
		    //                    layout: "table",
		    //                    width: 90,
		    //                    height: 30,
		    //                    items: [
		    //						{
		    //						    xtype: "button",
		    //						    text: "预览心愿",
		    //						    width: 80,
		    //						    handler: resetform
		    //						}
		    //					]
		    //                }
			]
		}
	]
});

Ext.onReady(function () {//初始化表单
    Ext.QuickTips.init(); //显示气球提示
    MainPanel.render("Main"); //填充元素到div（id）

});

function getIMG() {
    var img = parseInt(10 * Math.random());
    return img;
}

function resetform() {//重置表单
    MainPanel.getForm().reset();
}
function AjaxSubmit() {//提交事件
    if (!MainPanel.getForm().isValid())//验证表单是否非法
        return;
    //            if (MainPanel.getForm().findField('pwd').getValue() != ExtRegister.getForm().findField('pwd2').getValue()) {//验证密码匹配
    //                Ext.Msg.alert("提示", "两次密码输入不匹配，请重新输入");
    //                ExtRegister.getForm().findField('pwd').setValue("");
    //                ExtRegister.getForm().findField('pwd2').setValue("");
    //            }
    //获取发送参数
    var title = MainPanel.getForm().findField('title').getValue();
    var writer = MainPanel.getForm().findField('writer').getValue();
    var joiner = MainPanel.getForm().findField('joiner').getValue();
    var contact = MainPanel.getForm().findField('contact').getValue();
    var bgimage = getIMG(); //MainPanel.getForm().findField('bgimage').getValue();
    var beginDate = MainPanel.getForm().findField('beginDate').getValue();
    var endDate = MainPanel.getForm().findField('endDate').getValue();
    var content = MainPanel.getForm().findField('content').getValue();
    //var filename = MainPanel.getForm().findField('userfile').getValue();

    Ext.Ajax.request(//发出ajax请求，接收方式后台用Form！！
            {
            url: 'control/controler.ashx',
            success: function (response) {
                Ext.Msg.alert("      提交成功      ", "      心愿发布成功!      ");
                window.location = 'happywall.html';
            },
            failure: function (response) {
                Ext.Msg.alert("提交失败", "心愿发布失败");
            },
            params: { type: 'pubHeart', title: title, writer: writer, joiner: joiner, contact: contact, bgimage: bgimage, beginDate: beginDate, endDate: endDate, content: content, date: new Date() }
        }
            )
};
 