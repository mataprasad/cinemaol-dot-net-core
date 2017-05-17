using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using WebApplication.Services;
using WebApplication.Models;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace WebApplication.Data
{
    public class DbAccess
    {
        private string _connectionString = null;
        private SqliteConnection _connection = null;
        private GlobalOption _globalOption = null;

        public DbAccess(GlobalOption globalOption)
        {
            this._globalOption = globalOption;
            var sqliteConnectionStringBuilder =
            new SqliteConnectionStringBuilder(globalOption.DefaultConnectionString);
            sqliteConnectionStringBuilder.DataSource =
            Path.Combine(globalOption.ContentRootPath, sqliteConnectionStringBuilder.DataSource);
            this._connectionString = sqliteConnectionStringBuilder.ToString();
            sqliteConnectionStringBuilder = null;
        }

        public object ExecuteScalar(string query, Dictionary<string, object> parametes)
        {
            try
            {
                object data = null;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = query;
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    if (parametes != null && parametes.Count > 0)
                    {
                        foreach (var item in parametes)
                        {
                            selectCommand.Parameters.AddWithValue(item.Key, item.Value);
                        }
                    }
                    data = selectCommand.ExecuteScalar();
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<SelectListItem> GetDdlData(string query, int keyIndex = 0, int valueIndex = 0, Dictionary<string, object> parametes = null)
        {
            try
            {
                List<SelectListItem> data = new List<SelectListItem>();
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = query;
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    if (parametes != null && parametes.Count > 0)
                    {
                        foreach (var item in parametes)
                        {
                            selectCommand.Parameters.AddWithValue(item.Key, item.Value);
                        }
                    }
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new SelectListItem() { Text = reader.GetString(keyIndex), Value = reader.GetString(valueIndex) });
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string ConnectionString
        {
            get
            {
                return this._connectionString;
            }
        }

        public bool SpUpdateUserInfo(UserInfo obj)
        {
            try
            {
                bool data = false;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = @"
                        update UserInfo set User_Email=$User_Email,User_MobileNo=$User_MobileNo,
                        User_FName=$User_FName,User_LName=$User_LName,User_Add=$User_Add,
                        User_City=$User_City,User_State=$User_State,User_SQ=$User_SQ,User_SA=$User_SA 
                        where[User_Id]=$User_Id;
                        ";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    selectCommand.Parameters.AddWithValue("$User_Id", obj.User_Id);
                    selectCommand.Parameters.AddWithValue("$User_Email", obj.User_Email.ToLower());
                    selectCommand.Parameters.AddWithValue("$User_MobileNo", obj.User_MobileNo);
                    selectCommand.Parameters.AddWithValue("$User_FName", obj.User_FName.Trim());
                    selectCommand.Parameters.AddWithValue("$User_LName", obj.User_LName.Trim());
                    selectCommand.Parameters.AddWithValue("$User_Add", obj.User_Add.Trim());
                    selectCommand.Parameters.AddWithValue("$User_City", obj.User_City.Trim());
                    selectCommand.Parameters.AddWithValue("$User_State", obj.User_State);
                    selectCommand.Parameters.AddWithValue("$User_SQ", obj.User_SQ.Trim());
                    selectCommand.Parameters.AddWithValue("$User_SA", obj.User_SA.Trim());
                    if (selectCommand.ExecuteNonQuery() > 0)
                    {
                        data = true;
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListItem> GetHallList()
        {
            var data = GetDdlData("SELECT Hall_Id,Hall_No FROM HallInfo", 1, 0);
            data.Insert(0, new SelectListItem() { Value = "0", Text = "<-- SELECT HALL -->" });
            return data;
        }

        public List<SelectListItem> GetTimeList()
        {
            var data = GetDdlData("SELECT Time_StartTime FROM TimingInfo", 0, 0);
            data.Insert(0, new SelectListItem() { Value = "0", Text = "<-- SELECT Time -->" });
            return data;
        }

        public List<SelectListItem> GetMovieList()
        {
            var data = GetDdlData("SELECT Movie_Title FROM MovieInfo WHERE Movie_Status<>3", 0, 0);
            data.Insert(0, new SelectListItem() { Value = "0", Text = "<-- SELECT Movie -->" });
            return data;
        }
        public List<SelectListItem> GetMovieLanguages()
        {
            try
            {
                List<SelectListItem> data = new List<SelectListItem>();
                data.Add(new SelectListItem() { Text = "--SELECT Language--", Value = "0" });
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = "SELECT distinct Language_Name FROM MovieLanguageInfo order by Language_Name;";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new SelectListItem() { Text = reader.GetString(0), Value = reader.GetString(0) });
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<SelectListItem> GetMovieIndustries()
        {
            try
            {
                List<SelectListItem> data = new List<SelectListItem>();
                data.Add(new SelectListItem() { Text = "--SELECT Industry--", Value = "0" });
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = "SELECT distinct Industry_Name FROM MovieIndustryInfo order by Industry_Name;";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new SelectListItem() { Text = reader.GetString(0), Value = reader.GetString(0) });
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<SelectListItem> GetMovieStatuses()
        {
            try
            {
                List<SelectListItem> data = new List<SelectListItem>();
                data.Add(new SelectListItem() { Text = "--SELECT Status--", Value = "0" });
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = "SELECT MovieStatus_Id ,MovieStatus_Value FROM MovieStatusInfo order by MovieStatus_Value;";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new SelectListItem() { Value = reader.GetInt32(0).ToString(), Text = reader.GetString(1) });
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<SelectListItem> FillMovieList()
        {
            try
            {
                List<SelectListItem> data = new List<SelectListItem>();
                data.Add(new SelectListItem() { Text = "--SELECT--", Value = "0" });
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = "SELECT distinct Movie_Name FROM ShowInfo WHERE Show_Date>=date('now') and Movie_Name is not null";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new SelectListItem() { Text = reader.GetString(0), Value = reader.GetString(0) });
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListItem> FillDateList(String pMovieTitle)
        {
            try
            {
                List<SelectListItem> data = new List<SelectListItem>();
                data.Add(new SelectListItem() { Text = "--SELECT--", Value = "0" });
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = "SELECT distinct strftime('%d/%m/%Y',Show_Date) as ShowDate FROM ShowInfo WHERE Show_Date>=date('now') and Movie_Name=$Movie_Name";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    selectCommand.Parameters.AddWithValue("$Movie_Name", pMovieTitle);
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new SelectListItem() { Text = reader.GetString(0), Value = reader.GetString(0) });
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListItem> FillTimeList(String pMovieTitle, String pMovieDate)
        {
            try
            {
                List<SelectListItem> data = new List<SelectListItem>();
                data.Add(new SelectListItem() { Text = "--SELECT--", Value = "0" });
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = "SELECT distinct Show_StartTime FROM ShowInfo WHERE strftime('%d/%m/%Y',Show_Date)=$Show_Date and Movie_Name=$Movie_Name";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    selectCommand.Parameters.AddWithValue("$Show_Date", pMovieDate);
                    selectCommand.Parameters.AddWithValue("$Movie_Name", pMovieTitle);
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new SelectListItem() { Text = reader.GetString(0), Value = reader.GetString(0) });
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetShowId(VMSelectShowPost obj)
        {
            try
            {
                int data = -1;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = @"select Show_Id from ShowInfo
                        where strftime('%d/%m/%Y',Show_Date)=$Show_Date
                        and Movie_Name=$Movie_Name and Show_StartTime=$Show_StartTime";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    selectCommand.Parameters.AddWithValue("$Show_Date", obj.ddlShowDate);
                    selectCommand.Parameters.AddWithValue("$Movie_Name", obj.ddlShowMovie);
                    selectCommand.Parameters.AddWithValue("$Show_StartTime", obj.ddlShowTime);
                    data = Convert.ToInt32(selectCommand.ExecuteScalar());
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BookingHistory> SpBookingHistoryWithPaging(int User_Id, int pagesize, int pageno)
        {
            int min = ((pageno - 1) * pagesize) + 1;
            int max = min + (pagesize - 1);
            try
            {
                List<BookingHistory> data = new List<BookingHistory>();
                BookingHistory obj = null;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = @"
                    with y as (SELECT  ShowInfo.Show_Id,ShowInfo.Movie_Name,
                    TicketInfo.Ticket_Id,TicketInfo.Ticket_No,TicketInfo.User_Id,TicketInfo.Show_Date as'Show_Date',
                    TicketInfo.Show_Time,TicketInfo.Booking_Date as'Booking_Date','' as URL FROM TicketInfo JOIN ShowInfo ON TicketInfo.Show_Id=ShowInfo.Show_Id 
                    where TicketInfo.User_Id=$User_Id),
					x as (select  (select count(*) from y b  where a.Ticket_Id >= b.Ticket_Id) as SNO,*
                    from y a)
                    select *,0 AS RecordCount from x where x.SNo between $min and $max
                    union all
                    select 0,0,'',0,0,0,'','','','',COUNT(0) AS RecordCount from x";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    selectCommand.Parameters.AddWithValue("$User_Id", User_Id);
                    selectCommand.Parameters.AddWithValue("$min", min);
                    selectCommand.Parameters.AddWithValue("$max", max);
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obj = new BookingHistory();
                            obj.SNo = reader.GetInt32(0);
                            obj.Show_Id = reader.GetInt32(1).ToString();
                            obj.Movie_Name = reader.GetString(2);
                            obj.Ticket_Id = reader.GetInt32(3);
                            obj.Ticket_No = reader.GetInt32(4);
                            obj.User_Id = reader.GetInt32(5).ToString();
                            obj.Show_Date = reader.GetString(6);
                            obj.Show_Time = reader.GetString(7);
                            obj.Booking_Date = reader.GetString(8);
                            obj.URL = reader.GetString(9);
                            obj.RecordCount = reader.GetInt32(10);
                            if (!String.IsNullOrWhiteSpace(obj.Booking_Date))
                            {
                                obj.Booking_Date = DateTime.ParseExact(obj.Booking_Date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                            }
                            data.Add(obj);
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(Newtonsoft.Json.JsonConvert.SerializeObject(ex));
            }
        }
        public bool SpChangePassword(UserInfo obj)
        {
            try
            {
                bool data = false;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = "update UserInfo set User_LoginPassword=$User_LoginPassword where User_Id=$User_Id;";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    selectCommand.Parameters.AddWithValue("$User_LoginPassword", obj.User_LoginPassword);
                    selectCommand.Parameters.AddWithValue("$User_Id", obj.User_Id);
                    if (selectCommand.ExecuteNonQuery() > 0)
                    {
                        data = true;
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UserInfo SpCheckUser(string txtLoginId)
        {
            try
            {
                var user_type = "USER";
                txtLoginId = txtLoginId.Trim().ToLower();
                UserInfo data = null;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = @"select 
                        User_Id
                        ,User_LoginName
                        ,User_LoginPassword
                        ,User_Email
                        ,User_MobileNo
                        ,User_FName
                        ,User_LName
                        ,User_Add
                        ,User_City
                        ,User_State
                        ,User_SQ
                        ,User_SA
                        ,User_Type
                        ,User_IsActive
                        from UserInfo where User_LoginName=$User_LoginName and User_IsActive=1 and User_Type=$User_Type;";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    selectCommand.Parameters.AddWithValue("$User_LoginName", txtLoginId);
                    selectCommand.Parameters.AddWithValue("$User_Type", user_type);
                    using (var reader = selectCommand.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            data = new UserInfo();
                            data.User_Id = reader.GetInt32(0);
                            data.User_LoginName = reader.GetString(1);
                            data.User_LoginPassword = reader.GetString(2);
                            data.User_Email = reader.GetString(3);
                            data.User_MobileNo = reader.GetString(4);
                            data.User_FName = reader.GetString(5);
                            data.User_LName = reader.GetString(6);
                            data.User_Add = reader.GetString(7);
                            data.User_City = reader.GetString(8);
                            data.User_State = reader.GetString(9);
                            data.User_SQ = reader.GetString(10);
                            data.User_SA = reader.GetString(11);
                            data.User_Type = reader.GetString(12);
                            data.User_IsActive = reader.GetBoolean(13);
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserInfo SpUserLogin(VMLogin obj, bool forAdmin = false)
        {
            try
            {
                var user_type = "USER";
                if (forAdmin)
                {
                    user_type = "ADMIN";
                }
                obj.txtLoginId = obj.txtLoginId.Trim().ToLower();
                UserInfo data = null;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = @"select 
                        User_Id
                        ,User_LoginName
                        ,User_LoginPassword
                        ,User_Email
                        ,User_MobileNo
                        ,User_FName
                        ,User_LName
                        ,User_Add
                        ,User_City
                        ,User_State
                        ,User_SQ
                        ,User_SA
                        ,User_Type
                        ,User_IsActive
                        from UserInfo where User_LoginName=$User_LoginName and 
                        User_LoginPassword=$User_LoginPassword and User_IsActive=1 and User_Type=$User_Type;";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    selectCommand.Parameters.AddWithValue("$User_LoginName", obj.txtLoginId);
                    selectCommand.Parameters.AddWithValue("$User_LoginPassword", obj.txtLoginPass);
                    selectCommand.Parameters.AddWithValue("$User_Type", user_type);
                    using (var reader = selectCommand.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            data = new UserInfo();
                            data.User_Id = reader.GetInt32(0);
                            data.User_LoginName = reader.GetString(1);
                            data.User_LoginPassword = reader.GetString(2);
                            data.User_Email = reader.GetString(3);
                            data.User_MobileNo = reader.GetString(4);
                            data.User_FName = reader.GetString(5);
                            data.User_LName = reader.GetString(6);
                            data.User_Add = reader.GetString(7);
                            data.User_City = reader.GetString(8);
                            data.User_State = reader.GetString(9);
                            data.User_SQ = reader.GetString(10);
                            data.User_SA = reader.GetString(11);
                            data.User_Type = reader.GetString(12);
                            data.User_IsActive = reader.GetBoolean(13);
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<String> SelectSheats(int show_id)
        {
            try
            {
                List<String> data = new List<String>();
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = "select Sheat_No from TicketDetail where Show_Id=$Show_Id;";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    selectCommand.Parameters.AddWithValue("$Show_Id", show_id);
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(reader.GetString(0));
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<String> SpGetMoviesImageURL()
        {
            try
            {
                List<String> data = new List<String>();
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = "SELECT Movie_ImageURL FROM MovieInfo where Movie_Status=1 or Movie_Status=2;";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(reader.GetString(0));
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<String> GetAllStates()
        {
            try
            {
                List<String> data = new List<String>();
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = "select UPPER(State_Name) as State from StateInfo";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(reader.GetString(0));
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsUserExists(String txtUName)
        {
            try
            {
                txtUName = txtUName.Trim().ToLower();
                var data = false;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = "select 1 from UserInfo where User_LoginName=$User_LoginName and User_Type='USER'";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    selectCommand.Parameters.AddWithValue("$User_LoginName", txtUName);
                    if (selectCommand.ExecuteScalar() != null)
                    {
                        data = true;
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool RegisterUser(VMRegister obj)
        {
            try
            {
                obj.txtUName = obj.txtUName.Trim().ToLower();
                var data = false;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = @"
                    INSERT INTO UserInfo (User_LoginName,User_LoginPassword,User_Email,User_MobileNo,User_FName,User_LName,User_Add,User_City,User_State,User_SQ,User_SA,User_Type,User_IsActive) 
                    VALUES ($User_LoginName,$User_LoginPassword,$User_Email,$User_MobileNo,$User_FName,$User_LName,$User_Add,$User_City,$User_State,$User_SQ,$User_SA,'USER',1)
                    ";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    selectCommand.Parameters.AddWithValue("$User_LoginName", obj.txtUName);
                    selectCommand.Parameters.AddWithValue("$User_LoginPassword", obj.txtRePass);
                    selectCommand.Parameters.AddWithValue("$User_Email", obj.txtEmail);
                    selectCommand.Parameters.AddWithValue("$User_SQ", obj.txtSQ);
                    selectCommand.Parameters.AddWithValue("$User_MobileNo", obj.txtMobile);
                    selectCommand.Parameters.AddWithValue("$User_FName", obj.txtFName);
                    selectCommand.Parameters.AddWithValue("$User_LName", obj.txtLName);
                    selectCommand.Parameters.AddWithValue("$User_Add", String.Concat(obj.txtAdd1, " ", obj.txtAdd2));
                    selectCommand.Parameters.AddWithValue("$User_City", obj.txtCity);
                    selectCommand.Parameters.AddWithValue("$User_State", obj.ddlState);
                    selectCommand.Parameters.AddWithValue("$User_SA", obj.txtSA);
                    if (selectCommand.ExecuteNonQuery() > 0)
                    {
                        data = true;
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MovieInfo> GetRunningMovies()
        {
            try
            {
                List<MovieInfo> data = new List<MovieInfo>();
                MovieInfo obj = null;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = @"SELECT
                     Movie_Id
                    ,Movie_ImageURL
                    ,Movie_Status
                    ,Movie_Title
                    ,Movie_ReleaseDate
                    ,Movie_Director
                    ,Movie_Casts
                    ,Movie_Language
                    ,Movie_Industry
                    FROM MovieInfo where Movie_Status=1";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obj = new MovieInfo();
                            obj.Movie_Id = reader.GetInt32(0);
                            obj.Movie_ImageURL = reader.GetString(1);
                            obj.Movie_Status = reader.GetInt32(2);
                            obj.Movie_Title = reader.GetString(3);
                            obj.Movie_ReleaseDate = reader.GetString(4);
                            obj.Movie_Director = reader.GetString(5);
                            obj.Movie_Casts = reader.GetString(6);
                            obj.Movie_Language = reader.GetString(7);
                            obj.Movie_Industry = reader.GetString(8);
                            data.Add(obj);
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(Newtonsoft.Json.JsonConvert.SerializeObject(ex));
            }
        }

        public List<MovieInfo> GetUpCommingMovies()
        {
            try
            {
                List<MovieInfo> data = new List<MovieInfo>();
                MovieInfo obj = null;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = @"SELECT
                     Movie_Id
                    ,Movie_ImageURL
                    ,Movie_Status
                    ,Movie_Title
                    ,Movie_ReleaseDate
                    ,Movie_Director
                    ,Movie_Casts
                    ,Movie_Language
                    ,Movie_Industry
                    FROM MovieInfo where Movie_Status=2";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obj = new MovieInfo();
                            obj.Movie_Id = reader.GetInt32(0);
                            obj.Movie_ImageURL = reader.GetString(1);
                            obj.Movie_Status = reader.GetInt32(2);
                            obj.Movie_Title = reader.GetString(3);
                            obj.Movie_ReleaseDate = reader.GetString(4);
                            obj.Movie_Director = reader.GetString(5);
                            obj.Movie_Casts = reader.GetString(6);
                            obj.Movie_Language = reader.GetString(7);
                            obj.Movie_Industry = reader.GetString(8);
                            data.Add(obj);
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetDbTest()
        {
            System.Text.StringBuilder txt = new System.Text.StringBuilder();
            using (this._connection = new SqliteConnection(this._connectionString))
            {
                this._connection.Open();

                using (var transaction = this._connection.BeginTransaction())
                {
                    // var insertCommand = connection.CreateCommand();
                    // insertCommand.Transaction = transaction;
                    // insertCommand.CommandText = "INSERT INTO message ( text ) VALUES ( $text )";
                    // insertCommand.Parameters.AddWithValue("$text", "Hello, World!");
                    // insertCommand.ExecuteNonQuery();

                    var selectCommand = this._connection.CreateCommand();
                    selectCommand.Transaction = transaction;
                    selectCommand.CommandText = "SELECT SheatType_Value FROM SheatType";
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var message = reader.GetString(0);
                            // Console.WriteLine(message);
                            txt.Append(message);
                            txt.AppendLine();
                        }
                    }

                    transaction.Commit();
                }
            }
            return txt.ToString();
        }

        public ShowDetail SelectShow(string show_id)
        {
            try
            {
                ShowDetail data = null;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = @"select Show_Id,strftime('%d/%m/%Y',Show_Date) AS Show_Date,Show_StartTime,Movie_Name,(select h.Hall_No from HallInfo h
where Hall_Id=s.Hall_No) As Hall_No  from ShowInfo s where s.Show_Id=$Show_Id";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    selectCommand.Parameters.AddWithValue("$Show_Id", show_id);
                    using (var reader = selectCommand.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            data = new ShowDetail();
                            data.Hall_No = reader.GetString(4);
                            data.Movie_Name = reader.GetString(3);
                            data.Show_Date = reader.GetString(1);
                            data.Show_Id = reader.GetString(0);
                            data.Show_StartTime = reader.GetString(2);
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetTicketIdAndNo(out int id, out int no)
        {
            try
            {
                id = -1;
                no = -1;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = @"select ifnull(max(Ticket_id),0) as id,ifnull(max(Ticket_no),0) as no from TicketInfo";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    using (var reader = selectCommand.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            id = reader.GetInt32(0);
                            no = reader.GetInt32(1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private static object lock_object = new object();
        public bool BookTicket(Decimal totalCost, int showId, int userId, int sheatsCount, out int tktId, out int tktNo)
        {
            try
            {
                lock (lock_object)
                {
                    tktId = 0;
                    tktNo = 0;
                    GetTicketIdAndNo(out tktId, out tktNo);
                    var show = SelectShow(showId.ToString());
                    bool data = false;
                    using (this._connection = new SqliteConnection(this._connectionString))
                    {
                        this._connection.Open();

                        var selectCommand = new SqliteCommand();
                        selectCommand.Connection = this._connection;
                        selectCommand.CommandText = @"INSERT INTO TicketInfo(Ticket_No,User_Id,Show_Date,Show_Time,Show_Id,Ticket_Amount,IsPaid,Sheats_Count,Booking_Date,Ticket_id)
                        VALUES($Tkt_No,$User_Id,$ShowDate,$ShowTime,$Show_Id,$Ticket_Amount,'False',$Sheats_Count,$Booking_Date,$Ticket_id);
                        ";
                        selectCommand.CommandType = System.Data.CommandType.Text;
                        selectCommand.Parameters.AddWithValue("$Ticket_id", tktId + 1);
                        selectCommand.Parameters.AddWithValue("$Tkt_No", tktNo + 1);
                        selectCommand.Parameters.AddWithValue("$User_Id", userId);
                        selectCommand.Parameters.AddWithValue("$ShowDate", show.Show_Date);
                        selectCommand.Parameters.AddWithValue("$ShowTime", show.Show_StartTime);
                        selectCommand.Parameters.AddWithValue("$Show_Id", showId);
                        selectCommand.Parameters.AddWithValue("$Ticket_Amount", totalCost);
                        selectCommand.Parameters.AddWithValue("$Sheats_Count", sheatsCount);
                        selectCommand.Parameters.AddWithValue("$Booking_Date", DateTime.Now.ToString("yyyyMMdd"));
                        if (selectCommand.ExecuteNonQuery() > 0)
                        {
                            tktId = tktId + 1;
                            tktNo = tktNo + 1;

                            data = true;
                        }
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddTicketDetial(int showId, int ticketId, Hashtable hashtableSelectSheats)
        {
            try
            {
                foreach (string key in hashtableSelectSheats.Keys)
                {
                    using (this._connection = new SqliteConnection(this._connectionString))
                    {
                        this._connection.Open();

                        var selectCommand = new SqliteCommand();
                        selectCommand.Connection = this._connection;
                        selectCommand.CommandText = @"INSERT INTO TicketDetail(Ticket_id,Sheat_No,Sheat_Cost,Show_Id)
                            VALUES($Ticket_id,$Sheat_No,$Sheat_Cost,$Show_Id)";
                        selectCommand.CommandType = System.Data.CommandType.Text;
                        selectCommand.Parameters.AddWithValue("$Ticket_id", ticketId);
                        selectCommand.Parameters.AddWithValue("$Sheat_Cost", hashtableSelectSheats[key].ToString());
                        selectCommand.Parameters.AddWithValue("$Show_Id", showId);
                        selectCommand.Parameters.AddWithValue("$Sheat_No", key);
                        selectCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public VMBookTicket SpGetTicketHistoryDetial(int Ticket_Id)
        {
            try
            {
                VMBookTicket data = null;
                SelectListItem item = null;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = @"SELECT ShowInfo.Movie_Name,TicketInfo.Ticket_Id,TicketInfo.Ticket_No,TicketInfo.User_Id
                        ,TicketInfo.Show_Date as 'Show_Date' ,TicketInfo.Show_Time,
                        TicketInfo.Booking_Date as 'Booking_Date'
                        FROM TicketInfo INNER JOIN ShowInfo ON TicketInfo.Show_Id = ShowInfo.Show_Id where TicketInfo.Ticket_Id=$Ticket_Id;
                        SELECT Sheat_No,Sheat_Cost,Ticket_id FROM TicketDetail where Ticket_id=$Ticket_Id;";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    selectCommand.Parameters.AddWithValue("$Ticket_Id", Ticket_Id);
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data = new VMBookTicket();
                            data.Booking_Date = reader.GetString(6);
                            data.Movie_Name = reader.GetString(0);
                            data.Show_Date = reader.GetString(4);
                            data.Show_Time = reader.GetString(5);
                            data.Ticket_Id = reader.GetInt32(1).ToString();
                            data.Ticket_No = reader.GetInt32(2).ToString();
                            if (!String.IsNullOrWhiteSpace(data.Booking_Date))
                            {
                                data.Booking_Date = DateTime.ParseExact(data.Booking_Date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                            }
                            data.Sheats = new List<SelectListItem>();
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            item = new SelectListItem();
                            item.Text = reader.GetString(0);
                            item.Value = reader.GetInt32(1).ToString();
                            data.Sheats.Add(item);
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static object obj_lock_add_movie = new object();
        public bool SpAddNewMovie(MovieInfo obj, out string fileName)
        {
            try
            {
                lock (obj_lock_add_movie)
                {
                    var data = false;
                    var id = Convert.ToInt32(ExecuteScalar("SELECT ifnull(MAX(Movie_Id),0) FROM MovieInfo;", null)) + 1;
                    using (this._connection = new SqliteConnection(this._connectionString))
                    {
                        this._connection.Open();
                        fileName = String.Concat(id, obj.Movie_ImageURL);
                        var selectCommand = new SqliteCommand();
                        selectCommand.Connection = this._connection;
                        selectCommand.CommandText = @"
                    INSERT INTO MovieInfo (Movie_Id,Movie_ImageURL,Movie_Status,Movie_Title,
                    Movie_ReleaseDate,Movie_Director,Movie_Casts,Movie_Language,Movie_Industry) 
                    VALUES ($Movie_Id,$Movie_ImageURL,$Movie_Status,$Movie_Title,
                    $Movie_ReleaseDate,$Movie_Director,$Movie_Casts,$Movie_Language,$Movie_Industry)
                    ";
                        selectCommand.CommandType = System.Data.CommandType.Text;
                        selectCommand.Parameters.AddWithValue("$Movie_Id", id);
                        selectCommand.Parameters.AddWithValue("$Movie_ImageURL", "images/movieImages/" + fileName);
                        selectCommand.Parameters.AddWithValue("$Movie_Status", obj.Movie_Status);
                        selectCommand.Parameters.AddWithValue("$Movie_Title", obj.Movie_Title);
                        selectCommand.Parameters.AddWithValue("$Movie_ReleaseDate", obj.Movie_ReleaseDate);
                        selectCommand.Parameters.AddWithValue("$Movie_Director", obj.Movie_Director);
                        selectCommand.Parameters.AddWithValue("$Movie_Casts", obj.Movie_Casts);
                        selectCommand.Parameters.AddWithValue("$Movie_Language", obj.Movie_Language);
                        selectCommand.Parameters.AddWithValue("$Movie_Industry", obj.Movie_Industry);
                        if (selectCommand.ExecuteNonQuery() > 0)
                        {
                            data = true;
                        }
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool SpAddShowInfo(VMManageShow obj)
        {
            try
            {
                var data = false;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();
                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = @"
                    INSERT INTO ShowInfo
                    (Show_Date
                    ,Show_StartTime
                    ,Movie_Name
                    ,Hall_No)
                    VALUES
                    ($Show_Date
                    ,$Show_StartTime
                    ,$Movie_Name
                    ,$Hall_No)
                    ";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    selectCommand.Parameters.AddWithValue("$Show_Date", DateTime.ParseExact(obj.datepicker, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    selectCommand.Parameters.AddWithValue("$Show_StartTime", obj.ddlTime);
                    selectCommand.Parameters.AddWithValue("$Movie_Name", obj.ddlMovie);
                    selectCommand.Parameters.AddWithValue("$Hall_No", obj.ddlHall);
                    if (selectCommand.ExecuteNonQuery() > 0)
                    {
                        data = true;
                    }
                }
                return data;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MovieInfo> SpGetMoviesToRemove()
        {
            try
            {
                List<MovieInfo> data = new List<MovieInfo>();
                MovieInfo obj = null;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = @"
                        SELECT MovieInfo.Movie_Id, MovieInfo.Movie_Title, MovieStatusInfo.MovieStatus_Value,
                        Movie_ReleaseDate
                        FROM MovieInfo 
                        JOIN MovieStatusInfo on MovieInfo.Movie_Status=MovieStatusInfo.MovieStatus_Id 
                        where Movie_Status<>3;";
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obj = new MovieInfo();
                            obj.Movie_Id = reader.GetInt32(0);
                            obj.Movie_Title = reader.GetString(1);
                            obj.MovieStatus_Value = reader.GetString(2);
                            obj.Movie_ReleaseDate = reader.GetString(3);
                            data.Add(obj);
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool SpRemoveMovie(List<String> obj)
        {
            try
            {
                bool data = false;
                using (this._connection = new SqliteConnection(this._connectionString))
                {
                    this._connection.Open();

                    var selectCommand = new SqliteCommand();
                    selectCommand.Connection = this._connection;
                    selectCommand.CommandText = String.Format(@"UPDATE MovieInfo SET Movie_Status = 3 WHERE Movie_Id in ({0});", String.Join(",", obj));
                    selectCommand.CommandType = System.Data.CommandType.Text;
                    if (selectCommand.ExecuteNonQuery() > 0)
                    {
                        data = true;
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}