using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HeartData;

namespace MyHeart.Heart
{
    public partial class MessagePub : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string lastdate = DBTools.GetLastMessageDate();
            lblLastDate.Text = lastdate;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string date = txtDate.Value.ToString().Trim();
            string content = txtContent.Value.ToString().Trim();
            string writer = "juejue";
            if (ddlWriter.SelectedItem.Value == "gege")
            {
                writer = "gege";
            }
            if (DBTools.PubNewMessage(date, content, writer))
            {
                //success
                lblMessage.Text = date + " submit success! count:" + DBTools.GetMessageCount();
            }
            else
            {
                lblMessage.Text = date + " submit error!";
            }

            DateTime nextDay = Convert.ToDateTime(date);//.ToShortDateString();
            txtDate.Value = nextDay.AddDays(1).ToShortDateString().Trim();
        }

    }
}