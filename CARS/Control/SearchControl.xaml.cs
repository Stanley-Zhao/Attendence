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
using CARS.SourceCode;
using CARS.CARSService;
using System.Collections.ObjectModel;

namespace CARS.Control
{
    public delegate void CarsEventHandler(object sender, EventArgs e, bool onlyShowApply);

    public partial class SearchControl : UserControl
    {
        public event EventHandler RunAsEvent;
        private SearchData searcher = null;
        private User mUserRunAs = null;
        private bool busy = false;
        private List<User> supervisors = new List<User>();
        private List<User> applicants = new List<User>();

        public User UserRunAs
        {
            get { return mUserRunAs; }
        }

        public string SupervisorsID
        {
            get
            {
                string result = string.Empty;

                foreach (User user in supervisors)
                {
                    if (string.IsNullOrEmpty(result)) result = string.Format("'{0}'", user.PKEmployeeID);
                    else result = string.Format("{0},{1}", result, string.Format("'{0}'", user.PKEmployeeID));
                }

                return string.Format("({0})", result);
            }
        }

        RunAsControl rac = null;

        public SearchData Searcher
        {
            get { return searcher; }
            set { searcher = value; }
        }

        public bool Busy
        {
            get { return busy; }
            set { busy = value; }
        }

        private User mUser;

        #region Click Event
        public event EventHandler ClickApproveButton;
        public event EventHandler ClickRejectButton;
        public event EventHandler ClickSelectAllButton;
        public event EventHandler ClickClearAllButton;
        public event EventHandler ClickSearchButton;
        public event CarsEventHandler ClickRefreshButton;
        public event EventHandler ClickRefreshApplyButton;
        #endregion

        public SearchControl(CARSPage page, User rootManager)
        {
            InitializeComponent();

            SetPageAndUser(page, rootManager);

            chkShowAll.IsChecked = CarsConfig.Instance().ShowAllRecords;

            CARSServiceClient client = CARSServiceClientFactory.CreateCARSServiceClient();// leave this line, we cannot use a public static object here. Because it may already has handler of GetLeaveTypesCompleted 
            client.GetLeaveTypesCompleted += new EventHandler<GetLeaveTypesCompletedEventArgs>(client_GetLeaveTypesCompleted);
            client.GetLeaveTypesAsync();
        }

        private void client_GetLeaveTypesCompleted(object sender, GetLeaveTypesCompletedEventArgs e)
        {
            Logger.Instance().Log(MessageType.Information, "Get Leave Types Completed");
            if (ErrorHandler.Handle(e.Error))
                return;

            List<LeaveType> types = new List<LeaveType>();

            if (e.Result != null)
                types = ((ObservableCollection<LeaveType>)e.Result).ToList<LeaveType>();
            else
                Logger.Instance().Log(MessageType.Error, "Result is NULL");

            foreach (LeaveType type in types)
            {
                leaveType.Items.Add(new LeaveTypeValue(type));
            }
        }

        void rac_SelectRunAsEvent(object sender, EventArgs e)
        {
            mUserRunAs = rac.SelectedUser;
            RunAsEvent(sender, e);

            if (mUser.Employee.PKEmployeeID.ToString() == mUserRunAs.Employee.PKEmployeeID.ToString())
                runAsLabel.Visibility = System.Windows.Visibility.Collapsed;
            else
            {
                runAsLabel.Visibility = System.Windows.Visibility.Visible;
                runAsLabel.Content = "Run As: " + mUserRunAs.FirstName + " " + mUserRunAs.LastName;
            }
        }

        private void runasButton_Click(object sender, RoutedEventArgs e)
        {
            rac.Show();
        }

        private void approveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClickApproveButton != null && !busy)
            {
                busy = true;
                ClickApproveButton(sender, (EventArgs)e);
            }
        }

        private void rejectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClickRejectButton != null && !busy)
            {
                busy = true;
                ClickRejectButton(sender, (EventArgs)e);
            }
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClickRefreshButton != null && !busy)
            {
                busy = true;                
                    ClickRefreshButton(sender, e as EventArgs, CarsConfig.Instance().ShowAllRecords); // by default, show all
            }
        }

        private void refreshApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClickRefreshApplyButton != null && !busy)
            {
                busy = true;
                ClickRefreshApplyButton(sender, (EventArgs)e);
            }
        }

        private void clearAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClickClearAllButton != null)
            {
                ClickClearAllButton(sender, (EventArgs)e);
            }
        }

        private void selectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClickSelectAllButton != null)
            {
                ClickSelectAllButton(sender, (EventArgs)e);
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClickSearchButton != null)
            {
                ClickSearchButton(sender, (EventArgs)e);
            }
        }

        internal void SetPageAndUser(CARSPage page, User rootManager)
        {
            mUser = rootManager;
            if (page == CARSPage.ApproveLeave)
            {
                applyGrid.Visibility = System.Windows.Visibility.Collapsed;
                approvalGrid.Visibility = System.Windows.Visibility.Visible;
                historyGrid.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (page == CARSPage.LeaveHistory)
            {
                UpdateDropDownList();

                approvalGrid.Visibility = System.Windows.Visibility.Collapsed;
                historyGrid.Visibility = System.Windows.Visibility.Visible;
                applyGrid.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (page == CARSPage.ApplyLeave || page == CARSPage.PersonalInfo)
            {
                approvalGrid.Visibility = System.Windows.Visibility.Collapsed;
                historyGrid.Visibility = System.Windows.Visibility.Collapsed;
                applyGrid.Visibility = System.Windows.Visibility.Visible;
            }

            if (rac == null)
            {
                rac = new RunAsControl(mUser);
                rac.SelectRunAsEvent += new EventHandler(rac_SelectRunAsEvent);
            }
            else
                rac.SetRootManager(mUser);

            leaveType.SelectedIndex = leaveStatus.SelectedIndex = 0;
            start.SelectedDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1); // first day of current month
            end.SelectedDate = DateTime.Now;
        }

        private void UpdateDropDownList()
        {
            try
            {
                supervisors.Clear();
                applicants.Clear();
                supervisor.ItemsSource = null;
                applicant.ItemsSource = null;

                Employee none = new Employee();
                none.Email = "";
                none.FirstName = "All";
                none.Leaders = new ObservableCollection<Employee>();
                none.TeamMembers = new ObservableCollection<Employee>();

                //foreach (Employee employee in mUser.Employee.TeamMembers)
                //    applicants.Add(new User(employee));

                GetSupervisors(mUser, supervisors);
                GetTeamMembers();

                supervisors.Sort(new UserComparision());
                applicants.Sort(new UserComparision());
                supervisors.Insert(0, mUser);
                supervisors.Insert(0, new User(none));
                applicants.Insert(0, mUser);
                applicants.Insert(0, new User(none));

                supervisor.ItemsSource = supervisors;
                applicant.ItemsSource = applicants;

                supervisor.SelectedIndex = applicant.SelectedIndex = 0;
            }
            catch (Exception ex)
            {

                Message.Error("Sorry, an error!" + Environment.NewLine + ex.Message + Environment.NewLine + "Please refresh.");
            }
        }

        private void GetTeamMembers()
        {
            foreach (Employee employee in mUser.Employee.TeamMembers)
            {
                applicants.Add(new User(employee));
            }
        }

        private void GetSupervisors(User mUser, List<User> supervisors)
        {
            foreach (Employee employee in mUser.Employee.Leaders)
            {
                supervisors.Add(new User(employee));
                GetSupervisors(new User(employee), supervisors);
            }
        }

        private void end_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (start.SelectedDate.HasValue && end.SelectedDate.HasValue && start.SelectedDate.Value > end.SelectedDate.Value)
                start.SelectedDate = (DateTime?)end.SelectedDate.Value.AddDays(-1);
        }

        private void start_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (start.SelectedDate.HasValue && end.SelectedDate.HasValue && start.SelectedDate.Value > end.SelectedDate.Value)
                end.SelectedDate = (DateTime?)start.SelectedDate.Value.AddDays(1);
        }

        private void chkShowAll_Checked(object sender, RoutedEventArgs e)
        {
            if (!busy)
                CarsConfig.Instance().ShowAllRecords = chkShowAll.IsChecked ?? true;
        }

        private void chkShowAll_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!busy)
                CarsConfig.Instance().ShowAllRecords = chkShowAll.IsChecked ?? true;
        }
    }
}
