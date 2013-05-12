﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace _12306_Helper
{
    public partial class formPassengersEdit : Form
    {
        string TokenInit = "";
        string TokenEdit = "";
        CookieContainer _cookieContainer = new CookieContainer();
        FormShowStyle formStyle = new FormShowStyle();
        HTML_Translation translation = new HTML_Translation();
        ModifyPassengerAction modifyAction = new ModifyPassengerAction();
        OrderSelectAction orderSelectAction = new OrderSelectAction();
        static List<PassengersAllData> passenger;
        BindingList<PassengersAllData> _bindingList;
        public formPassengersEdit()
        {
            InitializeComponent();
            dgvPassenger.AutoGenerateColumns = false;
            passenger = new List<PassengersAllData>();
            _bindingList = new BindingList<PassengersAllData>(passenger);
            dgvPassenger.DataSource = _bindingList;
        }
        //获取联系人信息(编辑)
        private void formPassengersEdit_Load(object sender, EventArgs e)
        {
            formStyle.ShowForm(this.Handle, 500);
            _cookieContainer=formSelectTicket.cookieContainer;
            orderSelectAction.GetUnfinishedOrder((str) => { }, _cookieContainer);

            modifyAction.InitUsualPassenger12306((str)=>{
                if (str == "获取信息失败" || str == string.Empty)
                { MessageBox.Show("信息获取失败，请重试", "获取联系人信息(编辑)", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                TokenInit = System.Text.RegularExpressions.Regex.Match(str, "[0-9abcdefABCDEF]{32}").ToString();
                modifyAction.PostData = "pageIndex=0&pageSize=50&passenger_name=%E8%AF%B7%E8%BE%93%E5%85%A5%E6%B1%89%E5%AD%97%E6%88%96%E6%8B%BC%E9%9F%B3%E9%A6%96%E5%AD%97%E6%AF%8D";
                modifyAction.GetPagePassengerAll((str1) => {
                    translation.TranslationHtml(str1, (passengers) =>
                    {  
                        DeterMineCall(() =>
                        {
                            passenger = passengers;
                            _bindingList = new BindingList<PassengersAllData>(passenger);
                            dgvPassenger.DataSource = _bindingList;
                        });
                    });
                },_cookieContainer);

            },_cookieContainer);
        }

        private void lblClose_MouseEnter(object sender, EventArgs e)
        {
            lblClose.BackColor = Color.FromArgb(192, 192, 255);
        }

        private void lblTop_MouseDown(object sender, MouseEventArgs e)
        {
            formStyle.MoveForm(this.Handle);
        }

        private void lblMini_MouseEnter(object sender, EventArgs e)
        {
            lblMini.BackColor = Color.FromArgb(192, 192, 255);
        }

        private void lblMini_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void lblClose_MouseLeave(object sender, EventArgs e)
        {
            lblClose.BackColor = Color.FromArgb(128, 128, 255);
        }

        private void lblMini_MouseLeave(object sender, EventArgs e)
        {
            lblMini.BackColor = Color.FromArgb(128, 128, 255);
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            formStyle.HideForm(this.Handle, 500);
            this.Close();
        }

        private void DeterMineCall(MethodInvoker method)
        {
            if (InvokeRequired)
                Invoke(method);
            else
                method();
        }

        private void dgvPassenger_DataSourceChanged(object sender, EventArgs e)
        {
            //dgvPassenger.Refresh();
        }

        private void dgvPassenger_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                var pass=dgvPassenger.Rows[e.RowIndex].DataBoundItem as PassengersAllData;
                InitEditPassengerInfo(pass);
            }
        }

        private void InitEditPassengerInfo(PassengersAllData data)
        {
            modifyAction.PostData = "org.apache.struts.taglib.html.TOKEN=" + TokenInit
                        + "&name=" + translation.UtfEncode(data.passenger_name)
                        + "&card_type=" + data.card_type
                        + "&card_no=" + data.card_no
                        + "&passenger_type=" + data.passenger_type;
            modifyAction.InitModifyPassenger((str) => {
                if (str.IndexOf("保存") > -1)
                {
                    TokenEdit = System.Text.RegularExpressions.Regex.Match(str, "[0-9abcdefABCDEF]{32}").ToString();
                    UpdatePassengerInfo(data);
                }
                else
                {
                    MessageBox.Show("提示", "更新失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            },_cookieContainer);
        }

        private void UpdatePassengerInfo(PassengersAllData data)
        {
            modifyAction.PostData = string.Format("org.apache.struts.taglib.html.TOKEN={0}&name={1}&old_name={2}&gender={3}&sex_code={4}&born_date={5}&country_code={6}&card_type={7}&old_card_type={8}&card_no={9}&old_card_no={10}&psgTypeCode={11}&passenger_type={12}&mobile_no={13}&phone_no={14}&email={15}&address={16}&postalcode={17}&studentInfo.province_code={18}&studentInfo.school_code{19}&studentInfo.school_name={20}&studentInfo.department={21}&studentInfo.school_class={22}&studentInfo.student_no={23}&schoolSystemDefault={24}&studentInfo.school_system={25}&enterYearCode={26}&studentInfo.enter_year={27}&studentInfo.preference_card_no={28}&studentInfo.preference_from_station_name={29}&studentInfo.preference_from_station_code={30}&studentInfo.preference_to_station_name={31}&studentInfo.preference_to_station_code={32}",
                                                TokenEdit,translation.UtfEncode(data.passenger_name), translation.UtfEncode(data.old_name), data.gender, data.sex_code, data.born_date, data.country_code, data.card_type, data.old_card_type, data.card_no, data.old_card_no, data.psgTypeCode, data.passenger_type, data.mobile_no, data.phone_no, data.email, data.address, data.postalcode, data.studentInfo_province_code, data.studentInfo_school_code, data.studentInfo_school_name, data.studentInfo_department,data.studentInfo_school_class,data.studentInfo_student_no,data.schoolSystemDefault,data.studentInfo_school_system,data.enterYearCode,data.studentInfo_enter_year,data.studentInfo_preference_card_no,data.studentInfo_preference_from_station_name,data.studentInfo_preference_from_station_code,data.studentInfo_preference_to_station_name,data.studentInfo_preference_to_station_code);
            modifyAction.ModifyPassenger((str) => { 
                if(str.IndexOf("修改常用联系人成功")>-1)
                    MessageBox.Show("提示", "修改常用联系人成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("提示", "修改常用联系人失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
            },_cookieContainer);
        }
    }
}
