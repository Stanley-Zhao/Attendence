using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using CARS.SourceCode;
using CARS.Pages;
using CARS.CARSService;

/*
0 - No.
1 - Type
2 - Reason
3 - Start
4 - End
5 - Hours
6 - Status
7 - Applicant
8 - Approved By
9 - Button -- not now
10 - CheckBox -- 9
 */
namespace CARS.Control
{
    public partial class LeaveRecords : UserControl
    {
        ObservableCollection<LeaveItem> source = new ObservableCollection<LeaveItem>();
        private User mUser;
        private User mUserRunAs;
        private CARSPage mPage;
        private SearchControl sc;
        private Guid currentGUID = Guid.Empty;
        private DateTime currentFrozenDate = DateTime.MinValue;
        private double adjust = 180;

        public event SelectionChangedEventHandler SelectionChanged;
        public event EventHandler UpdateLeftUI;
        public event EventHandler RunAsEvent;

        public DateTime CurrentFrozenDate
        {
            set { currentFrozenDate = value; }
            get { return currentFrozenDate; }
        }

        public LeaveItem SelectedItem
        {
            get { return selectedItem; }
            //get { return (LeaveItem)records.SelectedItem; }
        }

        public User RunAsUser
        {
            get { return mUserRunAs; }
            set { mUserRunAs = value; }
        }

        public CARSPage CurrentPage
        {
            get { return mPage; }
            set
            {
                mPage = value;
                records.ItemsSource = null;
                RefreshUI();
            }
        }

        public LeaveRecords(CARSPage page, User pUser)
        {
            InitializeComponent();

            ClientInstance.Get().FindLeavesCompleted += new EventHandler<FindLeavesCompletedEventArgs>(client_FindLeavesCompleted);
            ClientInstance.Get().GetMyLeavesCompleted += new EventHandler<GetMyLeavesCompletedEventArgs>(client_GetMyLeavesCompleted);
            ClientInstance.Get().GetMyTeamLeavesCompleted += new EventHandler<GetMyTeamLeavesCompletedEventArgs>(client_GetMyTeamLeavesCompleted);

            mPage = page;

            if (pUser != null)
            {
                mUser = mUserRunAs = pUser;
                RefreshData(); // no change, current user, by default
            }

            RefreshUI();

            records.SelectionChanged += new SelectionChangedEventHandler(records_SelectionChanged);
        }

        private void RefreshUI()
        {
            if (mPage == CARSPage.ApplyLeave || mPage == CARSPage.PersonalInfo)
            {
                if (sc == null)
                {
                    CreateSearchControl();
                }
                else
                    sc.SetPageAndUser(mPage, mUser);

                /*
                0 - No. (0)
                1 - Type (100)
                2 - Reason (?)
                3 - Start (75)
                4 - End (75)
                5 - Hours (55)
                6 - Status (80)
                7 - Applicant (120) [hide]
                8 - Approved By (145)
                9 - Button (80) -- not now
                10 - CheckBox (15) -- 9 [hide]
                 */

                records.Columns[2].Visibility = System.Windows.Visibility.Visible; // reason
                records.Columns[2].Width = new DataGridLength(GetReasonWidth(950)); // reason
                records.Columns[6].Width = new DataGridLength(80); // status
                records.Columns[7].Visibility = System.Windows.Visibility.Collapsed; // applicant
                records.Columns[8].Width = new DataGridLength(145);  // approved by                
                records.Columns[8].Visibility = System.Windows.Visibility.Visible; // approved by
                records.Columns[9].Visibility = System.Windows.Visibility.Collapsed; // checkbox	
            }
            else if (mPage == CARSPage.ApproveLeave)
            {
                if (sc == null)
                {
                    CreateSearchControl();
                }
                else
                    sc.SetPageAndUser(mPage, mUser);

                /*
                0 - No. (0)
                1 - Type (100)
                2 - Reason (?)
                3 - Start (75)
                4 - End (75)
                5 - Hours (55)
                6 - Status (80)
                7 - Applicant (145)
                8 - Approved By (150) [hide]
                9 - Button (80) -- not now
                10 - CheckBox (15) -- 9
                 */
                records.Columns[2].Visibility = System.Windows.Visibility.Visible; // reason
                records.Columns[2].Width = new DataGridLength(GetReasonWidth(990)); // reason
                records.Columns[6].Width = new DataGridLength(80); // status
                records.Columns[7].Visibility = System.Windows.Visibility.Visible; // applicant
                records.Columns[7].Width = new DataGridLength(145);  // applicant                
                records.Columns[8].Visibility = System.Windows.Visibility.Collapsed; // approved by
                records.Columns[9].Visibility = System.Windows.Visibility.Visible; // checkbox	
            }
            else if (mPage == CARSPage.LeaveHistory)
            {
                if (sc == null)
                {
                    CreateSearchControl();
                }
                else
                    sc.SetPageAndUser(mPage, mUser);

                /*
                0 - No. (0)
                1 - Type (100)
                2 - Reason (170)
                3 - Start (75)
                4 - End (75)
                5 - Hours (55)
                6 - Status (75)
                7 - Applicant (145)
                8 - Approved By (145)
                9 - Button (80) -- not now
                10 - CheckBox (15) -- 9 [hide]
                 */
                records.Columns[2].Width = new DataGridLength(GetReasonWidth(1210)); ; // reason				
                records.Columns[6].Width = new DataGridLength(75); // status	
                records.Columns[7].Visibility = System.Windows.Visibility.Visible; // applicant
                records.Columns[7].Width = new DataGridLength(180); // applicant
                records.Columns[8].Visibility = System.Windows.Visibility.Visible; // approved by								
                records.Columns[8].Width = new DataGridLength(180); //  approved by
                records.Columns[9].Visibility = System.Windows.Visibility.Collapsed; // checkbox
            }
        }

        private double GetReasonWidth(int p)
        {
            double result = Application.Current.Host.Content.ActualWidth - p - adjust;
            result = result < 0 ? 0 : result;
            return result;
        }

        private void CreateSearchControl()
        {
            sc = new SearchControl(mPage, mUser);
            sc.RunAsEvent += new EventHandler(sc_RunAsEvent);
            sc.Width = 800;
            sc.Name = "SearchControl";
            Grid.SetRow(sc, 0);
            sc.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            sc.ClickSelectAllButton += new EventHandler(sc_ClickSelectAllButton);
            sc.ClickClearAllButton += new EventHandler(sc_ClickClearAllButton);
            sc.ClickApproveButton += new EventHandler(sc_ClickApproveButton);
            sc.ClickRejectButton += new EventHandler(sc_ClickRejectButton);
            sc.ClickSearchButton += new EventHandler(sc_ClickSearchButton);
            sc.ClickRefreshButton += new CarsEventHandler(sc_ClickRefreshButton);
            sc.ClickRefreshApplyButton += new EventHandler(sc_ClickRefreshButtonForApplying);
            LayoutRoot.Children.Add(sc);
        }

        void sc_ClickSearchButton(object sender, EventArgs e)
        {
            string typeID = string.Empty;
            if (sc.leaveType.SelectedIndex != 0)
                typeID = ((LeaveTypeValue)sc.leaveType.SelectedItem).TypeValue.PKLeaveTypeID.ToString();

            string status = ((ComboBoxItem)sc.leaveStatus.SelectedItem).Content.ToString();

            DateTime start = sc.start.SelectedDate.HasValue ? sc.start.SelectedDate.Value : DateTime.MinValue;
            DateTime end = sc.end.SelectedDate.HasValue ? sc.end.SelectedDate.Value : DateTime.MaxValue;
            if (end != DateTime.MaxValue) end = end.AddDays(1);
            ClientInstance.ShowSpinner("Finding");
            string supervisor = ((User)sc.supervisor.SelectedItem).UserName == "" ? "" : ((User)sc.supervisor.SelectedItem).PKEmployeeID.ToString();
            string applicant = ((User)sc.applicant.SelectedItem).UserName == "" ? "" : ((User)sc.applicant.SelectedItem).PKEmployeeID.ToString();

            ClientInstance.Get().FindLeavesAsync(supervisor, applicant, typeID, status, start, end, sc.SupervisorsID);
        }

        private void client_FindLeavesCompleted(object sender, FindLeavesCompletedEventArgs e)
        {
            Logger.Instance().Log(MessageType.Information, "Find Leaves Completed");
            if (ErrorHandler.Handle(e.Error))
                return;

            if (e.Result != null)
                FillData(e.Result);
            else
                Logger.Instance().Log(MessageType.Error, "Result is NULL");
        }

        void sc_RunAsEvent(object sender, EventArgs e)
        {
            RunAsUser = sc.UserRunAs;
            if (RunAsEvent != null)
                RunAsEvent(sender, e);
            RefreshData(CarsConfig.Instance().ShowAllRecords);
        }

        void sc_ClickRejectButton(object sender, EventArgs e)
        {
            bool noneSelection = true;
            List<LeaveItem> list = new List<LeaveItem>();
            foreach (LeaveItem item in source)
            {
                if (item.IsSelect)
                {
                    // check frozen date
                    if (item.Start < currentFrozenDate && item.Status != LeaveStatus.Applying)
                    {
                        Message.Information("You cannot reject " + item.Submitter + "'s " + item.TypeValue + " leave." + Environment.NewLine + "Because its Start Time:" + item.StartValue + " is before: " + currentFrozenDate.ToString("yyyy-MM-dd"));
                        return;
                    }

                    // If the record is already rejected, we don't need to reject it again.  Go to the next record.
                    if (item.Status == LeaveStatus.Rejected) continue;

                    list.Add(item);
                    noneSelection = false;
                }
            }

            if (!noneSelection)
            {
                RejectReason rr = new RejectReason(mUser, list);
                rr.RejectedLeaveEvent += new EventHandler(rr_RejectedLeaveEvent);
                rr.Show();
            }
            else
            {
                sc.Busy = false;
            }
        }

        void sc_ClickRefreshButtonForApplying(object sender, EventArgs e)
        {
            RefreshData(); // by default, show all
            sc.Busy = false;
        }

        void sc_ClickRefreshButton(object sender, EventArgs e, bool showAllRecords)
        {
            RefreshData(showAllRecords);
            sc.Busy = false;
        }

        void rr_RejectedLeaveEvent(object sender, EventArgs e)
        {
            if (UpdateLeftUI != null)
                UpdateLeftUI(sender, e);            
            RefreshData(CarsConfig.Instance().ShowAllRecords); 
            sc.Busy = false;
        }

        int count = 0;
        void sc_ClickApproveButton(object sender, EventArgs e)
        {
            foreach (LeaveItem leaveItem in source)
            {
                // check frozen date
                if (leaveItem.IsSelect && leaveItem.Start < currentFrozenDate)
                {
                    Message.Information("You cannot approve " + leaveItem.Submitter + "'s " + leaveItem.TypeValue + " leave." + Environment.NewLine + "Because its Start Time:" + leaveItem.StartValue + " is before: " + currentFrozenDate.ToString("yyyy-MM-dd"));
                    return;
                }
            }

            bool noneSelection = true;
            CARSServiceClient client = CARSServiceClientFactory.CreateCARSServiceClient();// leave this line, we cannot use a public static object here. Because it may already has handler of ApproveLeaveCompleted 
            client.ApproveLeaveCompleted += new EventHandler<ApproveLeaveCompletedEventArgs>(client_ApproveLeaveCompleted);

            foreach (LeaveItem item in source)
            {
                // If the record is already approved, we don't need to approve it again.  Go to the next record.
                if (item.IsSelect && item.Status != LeaveStatus.Accepted)
                {
                    noneSelection = false;
                    client.ApproveLeaveAsync(mUser.Employee.PKEmployeeID.ToString(), item.LeaveInfo.PKLeaveInfoID.ToString(), LeaveStatus.Accepted);
                    count++;
                }
            }

            if (noneSelection)
                sc.Busy = false;
        }

        private void client_ApproveLeaveCompleted(object sender, ApproveLeaveCompletedEventArgs e)
        {
            count--;
            if (count == 0)
            {
                Logger.Instance().Log(MessageType.Information, "Approve Leave Completed");
                if (ErrorHandler.Handle(e.Error))
                    return;

                if (UpdateLeftUI != null)
                    UpdateLeftUI(sender, e);
                RefreshData(CarsConfig.Instance().ShowAllRecords); 
                sc.Busy = false;

                Message.Information("Done! A notification email will be sent to your team member. You are in the CC list.");
            }
        }

        private void client_GetMyLeavesCompleted(object sender, GetMyLeavesCompletedEventArgs e)
        {
            Logger.Instance().Log(MessageType.Information, "Get My Leaves Completed");
            if (ErrorHandler.Handle(e.Error))
                return;

            if (e.Result != null)
                FillData(e.Result);
            else
                Logger.Instance().Log(MessageType.Error, "Result is NULL");
        }

        private void FillData(System.Collections.ObjectModel.ObservableCollection<LeaveInfo> observableCollection)
        {
            List<LeaveInfo> leaveInfos = (List<LeaveInfo>)observableCollection.ToList<LeaveInfo>();

            source.Clear();
            for (int i = 0; i < leaveInfos.Count; i++)
            {
                LeaveInfo li = (LeaveInfo)leaveInfos[i];
                LeaveItem item = new LeaveItem(li);
                item.Submitter = li.Submitter.FirstName + " " + li.Submitter.LastName;                
                //item.Index = (i + 1);
                source.Add(item);
            }

            records.ItemsSource = null;
            records.ItemsSource = source;

            int selectIndex = -1;
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i].LeaveInfo.PKLeaveInfoID == currentGUID)
                    selectIndex = i;
            }

            records.SelectedIndex = selectIndex;

            ClientInstance.HideSpinner();
        }

        public LeaveRecords() : this(CARSPage.ApplyLeave) { }

        public LeaveRecords(CARSPage page) : this(page, null) { }

        private void sc_ClickSelectAllButton(object sender, EventArgs e)
        {
            DoAllSelection(true);
        }

        private void sc_ClickClearAllButton(object sender, EventArgs e)
        {
            DoAllSelection(false);
        }

        private void DoAllSelection(bool value)
        {
            for (int i = 0; i < source.Count; i++)
            {
                source[i].IsSelect = value;
            }

            records.ItemsSource = null;
            records.ItemsSource = source;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if (((Button)sender).Content as string == "Cancel")
            //{
            //    source.Remove((LeaveItem)records.SelectedItem);
            //    records.ItemsSource = null;
            //    records.ItemsSource = source;
            //}
            //else
            //{
            ApplyToCancelLeave atcl = new ApplyToCancelLeave();
            atcl.Show();
            //}
        }

        private void LayoutRoot_LayoutUpdated(object sender, EventArgs e)
        {
            if (Application.Current.Host.Content.ActualWidth - 580 > 690)
            {
                this.Width = Application.Current.Host.Content.ActualWidth - 580;
            }
            else
            {
                this.Width = 690;
            }
            if (Application.Current.Host.Content.ActualHeight - 110 > 500)
            {
                this.MaxHeight = Application.Current.Host.Content.ActualHeight - 110;
            }
            else
            {
                this.MaxHeight = 500;
            }
        }

        public void RefreshData()
        {
            RefreshData(true); // by default, show all
        }

        public void RefreshData(bool showAllRecords)
        {
            if (records.SelectedItem != null)
                currentGUID = ((LeaveItem)records.SelectedItem).LeaveInfo.PKLeaveInfoID;
            if (mPage == CARSPage.ApplyLeave || mPage == CARSPage.PersonalInfo)
            {
                ClientInstance.ShowSpinner("Loading Leaves");
                ClientInstance.Get().GetMyLeavesAsync(mUserRunAs.Employee.PKEmployeeID.ToString());
            }
            else if (mPage == CARSPage.ApproveLeave)
            {
                ClientInstance.ShowSpinner("Loading Leaves");
                ClientInstance.Get().GetMyTeamLeavesAsync(mUserRunAs.Employee.PKEmployeeID.ToString(), showAllRecords);
            }
            else if (mPage == CARSPage.LeaveHistory)
            {
                records.ItemsSource = null;
            }
        }

        private void client_GetMyTeamLeavesCompleted(object sender, GetMyTeamLeavesCompletedEventArgs e)
        {
            Logger.Instance().Log(MessageType.Information, "Get My Team Completed");
            if (ErrorHandler.Handle(e.Error))
                return;

            if (e.Result != null)
                FillData(e.Result);
            else
                Logger.Instance().Log(MessageType.Error, "Result is NULL");
        }

        private void records_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            LeaveItem obj = e.Row.DataContext as LeaveItem;

            if (obj.Status == LeaveStatus.Applying)
            {
                e.Row.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x33, 0xFF));
            }
            else
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        public static T FindUpVisualTree<T>(DependencyObject initial) where T : DependencyObject
        {
            DependencyObject current = initial;

            while (current != null && current.GetType() != typeof(T))
            {
                current = VisualTreeHelper.GetParent(current);
            }
            return current as T;
        }

        private LeaveItem selectedItem = null;
        private void records_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = (LeaveItem)records.SelectedItem;

            if (SelectionChanged != null)
            {
                SelectionChanged(sender, e);
            }
        }

        private void records_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (CurrentPage == CARSPage.ApproveLeave)
            {
                DataGridRow row = FindUpVisualTree<DataGridRow>(e.OriginalSource as DependencyObject);
                //get the data object of the row               
                if (row != null && row.DataContext is LeaveItem)
                {
                    //toggle the IsSelected value  
                    (row.DataContext as LeaveItem).IsSelect = !(row.DataContext as LeaveItem).IsSelect;
                }

                records.ScrollIntoView(records.SelectedItem, records.Columns[0]);
            }
        }
    }
}
