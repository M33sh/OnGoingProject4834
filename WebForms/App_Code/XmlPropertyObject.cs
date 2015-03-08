using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Xml.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WebForms.App_Code
{
    public interface IXmlPropertyObject
    {
        // Required Properties - These are MetaData Properties

        int Id { get; set; }  // Every XmlPropertyObject is identified by a unique ID
        int ParentId { get; set; } // Optional Parent - references a parent object
        int ZOrder { get; set; }

        string Class { get; }
        string Name { get; set; }
        string Description { get; set; }

        // Keeps track of usage
        string OwnerUserId { get; set; }
        string CreatedDate { get; set; }

        string LastEditedUserId { get; set; }
        string LastEditedDate { get; set; }

        // Required Methods
        void SetId(int id);

        // There needs to be a way to convert the 
        // XML Data to and from the properties associated with the 
        string XmlSerialize();
        void XmlDeserialize(string xml);

        string ShortClassName();
        string FullClassName();

        // Methods to send data to/from database

        void Select();
        void Select(int id);

        int Insert(); // Answers the ID

        void Update();
        void Delete();

        // Other methods called externally

        string GetValue(string propertyName);
        void SetValue(string propertyName, string value);

        string getPropertyFromXml(string property, string xml);
        void setPropertyFromXml(string property, string xml);

        // For generating reports
        //string AsHtmlTable();
        //bool CanPlot();
    }

    public interface IUserControlPropertyInput
    {
        Dictionary<string, string> InputProperties { get; set; }
        Dictionary<string, string> InputPropertyValues { get; set; }

        string GetPropertyValue(string name);
        void SetPropertyValue(string name, string value);
    }

    public abstract class XmlPropertyObject : IXmlPropertyObject
    {
        public static string UserAnonymous = "Anonymous";
        public static string UserGlobal = "Global";

        #region Properties

        public int Id { get; set; }
        public int ParentId { get; set; }
        public int ZOrder { get; set; }

        public string Class
        {
            get { return ShortClassName(); }
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public string XmlData { get; set; }

        public string OwnerUserId { get; set; }
        public string CreatedDate { get; set; }
        public string LastEditedUserId { get; set; }
        public string LastEditedDate { get; set; }

        public int Version { get; set; }

        #endregion

        #region Constructors

        protected XmlPropertyObject() { }

        protected XmlPropertyObject(int ID)
        {
            Id = ID;
            Select(ID);
        }

        protected XmlPropertyObject(string xml)
        {
            XmlDeserialize(xml);
        }

        #endregion

        #region Miscellaneous Methods

        public string FullClassName()
        {
            return this.GetType().ToString();
        }

        public string ShortClassName()
        {
            string fc = this.GetType().ToString();
            while (fc.Contains("."))
            {
                fc = fc.Substring(fc.IndexOf(".") + 1);
            }
            return fc;
        }

        public void SetId(int ID)
        {
            Id = ID;
            Select(ID);
        }

        public bool CanPlot()
        {
            return false;
        }

        #endregion

        #region Array Functions
        public List<string> AsListAtIndex(int index, string property)
        {
            // property = Multi-line property list delimited \n - columns delimited by \t
            // returns a single column specified by index

            string[] s = SplitOnNewLine(property);
            List<string> l = new List<string>();

            foreach (string row in s)
            {
                string[] cells = SplitOnTab(row);
                l.Add(cells[index]);
            }
            return l;
        }

        public string FullRowAtIndex(int index, string value, string property)
        {
            // property = Multi-line property list delimited \n - columns delimited by \t
            // returns a single column specified by index

            string[] s = SplitOnNewLine(property);

            foreach (string row in s)
            {
                string[] cells = SplitOnTab(row);
                if (cells[index] == value) return row;
            }
            return string.Empty;
        }

        #endregion

        #region XML Serialize

        public string XmlSerialize()
        {
            string returnValue = String.Empty;
            string shortClassName = ShortClassName();

            while (shortClassName.Contains("."))
            {
                shortClassName = shortClassName.Substring(shortClassName.IndexOf(".") + 1);
            }

            returnValue += "<" + shortClassName + ">\n";
            returnValue += "<FullClassName>" + FullClassName() + "</FullClassName> \n";
            foreach (var property in this.GetType().GetProperties())
            {
                returnValue += " <" + property.Name + " type= '" + property.PropertyType.Name + "'>";
                switch (property.PropertyType.Name)
                {
                    case "Dictionary`2":
                        returnValue += "\n" + DictionaryValuesAsXml(property);
                        break;
                    case "List`1":
                        returnValue += ListValuesAsXml(property);
                        break;
                    case "String[]":
                        returnValue += StringArrayAsXml(property);
                        break;
                    case "Int32[]":
                        returnValue += IntegerArrayAsXml(property);
                        break;
                    default:
                        returnValue += HttpUtility.HtmlEncode(GetValue(property));
                        break;
                }

                returnValue += "</" + property.Name + ">";
                returnValue += "\n";
            }

            returnValue += "</" + shortClassName + ">";
            return returnValue;
        }

        public string DictionaryValuesAsXml(PropertyInfo pi)
        {
            // This method handles ONLY <string, string> dictionary
            // It answers the Dictionary as XML, The Dictionary keys
            // MUST be legal XML element names - http://www.w3schools.com/xml/xml_elements.asp 
            // It returns a string that is XML

            // Ensure that different dictionary types are not attempted
            if (pi.GetValue(this).ToString() == "System.Collections.Generic.Dictionary`2[System.String,System.String]")
            {
                return DictionaryStringStringValuesAsXml(pi);
            }
            return String.Empty;
        }

        private string DictionaryStringStringValuesAsXml(PropertyInfo pi)
        {
            Dictionary<string, string> dict = (Dictionary<string, string>)pi.GetValue(this, null);

            string s = string.Empty;
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                s += "<" + kvp.Key + ">";
                s += HttpUtility.HtmlEncode(kvp.Value);
                s += "</" + kvp.Key + ">\n";
            }
            return s;

        }

        public string ListValuesAsXml(PropertyInfo pi)
        {
            // This will return a string - that are the elements of the List that 
            // Is the property. It uses tab delimiting to determine the elements in the 
            // List

            if (pi.GetValue(this).ToString() == "System.Collections.Generic.List`1[System.Int32]")
            {
                return ListIntegerValuesAsXml(pi);
            }

            if (pi.GetValue(this).ToString() == "System.Collections.Generic.List`1[System.String]")
            {
                return ListStringValuesAsXml(pi);
            }
            return String.Empty;
        }


        private string ListIntegerValuesAsXml(PropertyInfo pi)
        {
            List<int> list = (List<int>)pi.GetValue(this, null);
            string s = String.Empty;
            foreach (int value in list)
            {
                if (value != null) s += Convert.ToString(value) + "\t";
            }
            s = s.Remove(s.Length - 1);
            return s;
        }

        private string ListStringValuesAsXml(PropertyInfo pi)
        {
            List<string> list = (List<string>)pi.GetValue(this, null);
            string s = String.Empty;
            foreach (string value in list)
            {
                if (value != null) s += HttpUtility.HtmlEncode(value.Replace("\t", "")) + "\t";
            }
            s = s.Remove(s.Length - 1);
            return s;

        }
        public string StringArrayAsXml(PropertyInfo pi)
        {
            // String Arrays are new tab delimited
            String[] array = (String[])pi.GetValue(this, null);
            string s = String.Empty;

            foreach (string value in array)
            {
                if (value != null) s += HttpUtility.HtmlEncode(value.Replace("\t", "")) + "\t";
            }
            s = s.Remove(s.Length - 1);
            return s;
        }

        public string IntegerArrayAsXml(PropertyInfo pi)
        {
            // String Arrays are new tab delimited
            Int32[] array = (Int32[])pi.GetValue(this, null);
            string s = String.Empty;

            foreach (int value in array)
            {
                if (value != null) s += Convert.ToString(value) + "\t";
            }
            s = s.Remove(s.Length - 1);
            return s;
        }

        public string GetValue(string propertyName)
        {
            // Answers the Value given the name of the property
            PropertyInfo pi = this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            return GetValue(pi);
        }

        public string GetValue(PropertyInfo pi)
        {
            // Answers the value of the Property from PropertyInfo
            if (pi == null) return string.Empty;
            if (pi.GetValue(this) == null) return String.Empty;
            return Convert.ToString(pi.GetValue(this));
        }

        public string GetValue(PropertyInfo pi, int places)
        {
            // Specifically to deal with numeric formats
            string formatString = "{0:N" + places.ToString().Trim() + "}";
            return String.Format(formatString, pi.GetValue(this));
        }

        public string GetValue(ArrayList al, int places)
        {
            string s = "\n";
            string formatString = "{0:N" + places.ToString().Trim() + "}";

            foreach (Double d in al)
            {
                s += String.Format(formatString, d) + "\n";
            }
            return s;
        }

        #endregion

        #region XML Deserialize
        // Deserialize retrieves values from XML and
        // Fills in properties
        public void XmlDeserialize(string xml)
        {
            //Deserialize(xml);
            var doc = XDocument.Parse(xml);
            foreach (XElement xe in doc.Root.Elements())
            {
                SetValue(xe);
            }
        }

        public void SetValue(XElement xe)
        {
            // If the type attribute is set - then we 
            if (xe.HasAttributes)
            {
                XAttribute xa = xe.FirstAttribute;
                if (xa.Name == "type")
                {
                    switch (xa.Value)
                    {
                        case "List`1":
                            SetListValues(xe.Name.ToString(), xe.Value);
                            break;
                        case "Dictionary`2":
                            SetDictionaryValues(xe.Name.ToString(), xe.ToString());
                            break;
                        case "String[]":
                            SetStringArrayValues(xe.Name.ToString(), xe.Value);
                            break;
                        case "Int32[]":
                            SetIntegerArrayValues(xe.Name.ToString(), xe.Value);
                            break;

                        default:
                            SetValue(xe.Name.ToString(), HttpUtility.HtmlDecode(xe.Value));
                            break;
                    }
                }
            }
            else
            {
                SetValue(xe.Name.ToString(), HttpUtility.HtmlDecode(xe.Value));
            }
        }

        public void SetDictionaryValues(string propertyName, string xml)
        {
            PropertyInfo pi = this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (null != pi && pi.CanWrite) //&& value != String.Empty
            {
                Dictionary<string, string> d1 = XmlAsDictionary(xml);
                Dictionary<string, string> d2 = pi.GetValue(this, null) as Dictionary<string, string>;

                foreach (KeyValuePair<string, string> kvp in d1)
                {
                    d2[kvp.Key] = kvp.Value;
                }
            }
        }

        private void SetListValues(string propertyName, string values)
        {
            PropertyInfo pi = this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (pi.GetValue(this).ToString() == "System.Collections.Generic.List`1[System.Int32]")
            {
                SetListIntegerValues(pi, values);
            }

            if (pi.GetValue(this).ToString() == "System.Collections.Generic.List`1[System.String]")
            {
                SetListStringValues(pi, values);
            }

        }

        private void SetListStringValues(PropertyInfo pi, string values)
        {
            if (null != pi && pi.CanWrite) //&& value != String.Empty
            {
                List<string> d1 = AsListString(values);
                List<string> d2 = pi.GetValue(this, null) as List<string>;

                foreach (string s in d1)
                {
                    d2.Add(HttpUtility.HtmlDecode(s));
                }
            }
        }

        private void SetListIntegerValues(PropertyInfo pi, string values)
        {
            if (null != pi && pi.CanWrite) //&& value != String.Empty
            {
                List<int> d1 = AsListInteger(values);
                List<int> d2 = pi.GetValue(this, null) as List<int>;

                foreach (int i in d1)
                {
                    d2.Add(i);
                }
            }
        }

        private void SetStringArrayValues(string propertyName, string values)
        {
            PropertyInfo pi = this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (null != pi && pi.CanWrite) //&& value != String.Empty
            {
                String[] d1 = AsStringArray(values, "\t");
                String[] d2 = pi.GetValue(this, null) as String[];

                int i = 0;
                foreach (string s in d1)
                {
                    d2[i] = HttpUtility.HtmlDecode(s);
                    i++;
                }
            }
        }

        public void SetIntegerArrayValues(string propertyName, string values)
        {
            PropertyInfo pi = this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (null != pi && pi.CanWrite) //&& value != String.Empty
            {
                String[] d1 = AsStringArray(values, "\t");
                Int32[] d2 = pi.GetValue(this, null) as Int32[];

                int i = 0;
                foreach (string s in d1)
                {
                    d2[i] = Convert.ToInt32(s);
                    i++;
                }
            }
        }

        public void SetValue(string propertyName, string value)
        {
            PropertyInfo pi = this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (null != pi && pi.CanWrite) //&& value != String.Empty
            {
                SetValue(pi, value);
            }
        }
        public void SetValue(PropertyInfo pi, string value)
        {
            var numerics = new List<string> { "Int32", "Int64", "Double", "Single" };
            if (numerics.Contains(pi.PropertyType.Name) && (value == String.Empty))
            {
                value = "0";
            }

            try
            {
                switch (pi.PropertyType.Name)
                {
                    default:
                        pi.SetValue(this, Convert.ChangeType(value, pi.PropertyType), null);
                        break;
                }

            }
            catch
            {

                // default is to do nothing;
            }
        }

        public void ClearProperties()
        {
            // Clears values of all properties of object

            foreach (var property in this.GetType().GetProperties())
            {
                SetValue(property.Name, String.Empty);
            }
        }

        public void setPropertyFromXml(string property, string xml)
        {
            string value = getPropertyFromXml(property, xml);
            SetValue(property, value);
        }

        public string getPropertyFromXml(string property, string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            // Gets the value fom XML
            try { return doc.Root.Element(property).Value; }
            catch { return String.Empty; }
        }

        public Dictionary<string, string> XmlAsDictionary(string xml)
        {
            // Answers a dictionary from the XML
            var doc = XDocument.Parse(xml);
            Dictionary<string, string> dict = new Dictionary<string, string>();

            if (dict != null) dict.Clear();
            foreach (XElement xe in doc.Root.Elements())
            {
                dict.Add(xe.Name.ToString(), HttpUtility.HtmlDecode(xe.Value));
            }
            return dict;
        }
        #endregion

        #region Database Operations Save
        public void Save()
        {
            if (Id == 0)
            {
                Insert();
            }
            else
            {
                Update();
            }
        }

        public void Save(string name)
        {
            // used when needing distinct name
            int n = CountForName(name);
            Name = name;
            if (n == 0)
            {
                Insert();
            }
            else
            {
                if (Id != 0) Update();
            }
        }

        public void SaveWithDistinctName(string name)
        {
            // used when needing distinct name
            int n = CountForName(name);
            Name = name;
            if (n == 0)
            {
                Insert();
            }
        }

        #endregion

        #region Database Operations Select

        public void Select()
        {
            if (Id == 0) return;
            Select(Id);
        }

        public void Select(int id)
        {
            //Fills out object based on ID

            DataTable dt = XmlObjectDatabase.Select(id);
            if (!Select(dt)) return;

            Id = id;
        }

        public void Select(string name)
        {
            // Returns the object given Name
            // Brings back a single response - use when name is unique
            // If multiple entries for name - will alway bring index 0

            DataTable dt = XmlObjectDatabase.Select(name);
            if (!Select(dt)) return;

            Id = Convert.ToInt32(FieldAsStringFromDataTable(dt, "id"));
            Name = name;
        }

        private bool Select(DataTable dt)
        {
            if (dt.Rows.Count == 0) return false;

            XmlData = FieldAsStringFromDataTable(dt, "XmlData");
            XmlDeserialize(XmlData);
            XmlData = String.Empty; // Prevent duplicate saving of XML, always generate dynamically

            string pid = FieldAsStringFromDataTable(dt, "ParentId");
            ParentId = Convert.ToInt32((pid == "") ? "0" : pid);

            string zo = FieldAsStringFromDataTable(dt, "ZOrder");
            ZOrder = Convert.ToInt32((zo == "") ? "0" : pid);


            Name = FieldAsStringFromDataTable(dt, "Name");
            Description = FieldAsStringFromDataTable(dt, "Description");
            OwnerUserId = FieldAsStringFromDataTable(dt, "OwnerUserId");
            CreatedDate = FieldAsStringFromDataTable(dt, "CreatedDate");
            LastEditedUserId = FieldAsStringFromDataTable(dt, "LastEditedUserId");
            LastEditedDate = FieldAsStringFromDataTable(dt, "LastEditedDate");
            Version = Convert.ToInt32(FieldAsStringFromDataTable(dt, "Version"));

            return true;
        }

        private string FieldAsStringFromDataTable(DataTable dt, string fieldName)
        {
            try
            {
                if (dt.Rows[0][fieldName] == null) return String.Empty;

                return dt.Rows[0][fieldName].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public DataTable SelectAllForOwnerUserId(string ownerId)
        {
            return XmlObjectDatabase.SelectAll(ownerId, ShortClassName());
        }

        public DataTable SelectAllForOwnerUserId()
        {
            return XmlObjectDatabase.SelectAll(OwnerUserId, ShortClassName());
        }

        public DataTable SelectAllForGlobalUser()
        {
            return XmlObjectDatabase.SelectAll(UserGlobal, ShortClassName());
        }

        public DataTable SelectAllForClass()
        {
            return XmlObjectDatabase.SelectAllForClass(ShortClassName());
        }

        #endregion

        #region Database Operations Counts
        public int CountForOwner()
        {
            return XmlObjectDatabase.SelectCount(OwnerUserId, ShortClassName());
        }

        public int CountForName(string name)
        {
            return XmlObjectDatabase.SelectCountForName(name, ShortClassName());
        }

        public int CountForOwner(string ownerId)
        {
            return XmlObjectDatabase.SelectCount(ownerId, ShortClassName());
        }

        #endregion

        #region Database Operations Search

        public DataTable SearchByName(string name)
        {
            return XmlObjectDatabase.SearchName(name, ShortClassName());
        }

        #endregion

        #region Database Operations Insert
        public int Insert()
        {
            // This will return the identity of the inserted object
            // as a string
            string ret = XmlObjectDatabase.Insert(this);
            if (ret != String.Empty) Id = Convert.ToInt32(ret);
            return Id;
        }

        public string InsertGlobal()
        {
            string owner = OwnerUserId;
            OwnerUserId = UserGlobal;
            string ret = XmlObjectDatabase.Insert(this);
            OwnerUserId = owner;
            return ret;
        }
        #endregion

        #region Database Operations Update

        public void Update()
        {
            if (Id == 0)
                Insert();
            else
                XmlObjectDatabase.Update(this);
        }
        #endregion

        #region Database Operations Delete

        public void Delete()
        {
            if (Id == 0) return;
            Delete(Id);
        }

        public static string Delete(int ID)
        {
            //    Deletes database entry based on Id
            return XmlObjectDatabase.Delete(Convert.ToInt32(ID));
        }

        public static string Delete(int ID, string user)
        {
            if (GetOwner(ID) != user) return "Not Owner of Object, Cannot Delete.";
            return Delete(ID);
        }

        public static string Delete(string ID, string user)
        {
            return Delete(Convert.ToInt32(ID), user);
        }

        public static string GetOwner(int ID)
        {
            // ANswers the owner of the current object based on ID
            return XmlObjectDatabase.SelectOwner(ID);
        }

        #endregion

        #region User Interface Methods

        //public string AsHtmlTable()
        //{
        //    string s = "<table>";
        //    foreach (KeyValuePair<string, string> pair in PropertyLabels())
        //    {
        //        s += "<tr>";
        //        s += "<td>" + pair.Value + "</td>";
        //        string v = GetValue(pair.Key);
        //        s += "<td>" + v + "</td>";
        //        s += "</tr>";
        //    }
        //    s += "</table>";
        //    return s;
        //}

        public static string AsHtmlTable(string[] header, string values)
        {
            // This accepts a string that is tab delimited by columns, new line delimited for rows
            // String array header specifies headers

            string s = "<table style='tableReport'>";
            s += "<tr style='tableReportHeaderRow'>";
            foreach (string h in header)
            {
                s += "<td style='tableReportHeaderCell'>";
                s += h;
                s += "</td>";
            }
            s += "</tr>";

            // Now the data
            if (values == null)
            {
                s += "</table>";
                return s;
            }

            string[] rows = values.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string row in rows)
            {
                s += "<tr style='tableReportRow'>";
                string[] columns = row.Split(new String[] { "\t" }, StringSplitOptions.None);
                foreach (string column in columns)
                {
                    s += "<td style='tableReportCell'>";
                    s += column;
                    s += "</td>";
                }
                s += "</tr>";
            }
            s += "</table>";
            return s;
        }

        public void GetPropertiesFromUserControl(Dictionary<string, string> d, IUserControlPropertyInput uc)
        {
            // Sets the properties from a User Control that implements IUserControlPropertyInput
            // d - Dictionary of properties to set.

            foreach (KeyValuePair<string, string> p in d)
            {
                this.SetValue(p.Key, uc.GetPropertyValue(p.Key));
            }
        }

        public void SetPropertValuesInUserControl(Dictionary<string, string> d, IUserControlPropertyInput uc)
        {
            foreach (KeyValuePair<string, string> p in d)
            {
                uc.SetPropertyValue(p.Key, GetValue(p.Key).ToString());
            }
        }

        //public int PropertyDecimalPlaces(string propertyName)
        //{
        //    try { return PropertyDecimalPlaces()[propertyName]; }
        //    catch { return 0; }
        //}
        public Dictionary<String, String> PropertyValues()
        {
            var d = this.GetType().GetProperties().ToDictionary(property => property.Name, property => Convert.ToString(property.GetValue(this)));
            return d;
        }

        public string ArrayListAsString(ArrayList al, int places)
        {
            if (al == null) return String.Empty;
            string s = String.Empty;
            string FormatString = "{0:N" + places.ToString().Trim() + "}";

            foreach (double d in al)
            {
                s += String.Format(FormatString, d) + "\n";
            }
            return s;
        }

        public string AsCode(string objectName = "")
        {
            string s = String.Empty;
            string fc = ShortClassName();

            while (fc.Contains("."))
            {
                fc = fc.Substring(fc.IndexOf(".") + 1);
            }

            foreach (var property in this.GetType().GetProperties())
            {
                if (objectName != "") s += objectName + ".";
                s += property.Name + " = ";
                if (property.PropertyType.Name == "String")
                {
                    string t = GetValue(property).Replace("\t", "\\t");
                    t = t.Replace("\n", "\\n");
                    if (t != "") s += "\"" + t + "\"";
                }
                else
                {
                    s += GetValue(property);
                }
                s += ";\n";
            }

            return s;
        }

        #endregion

        #region Static Helper Methods

        //public static string AddToString(string property, string member)
        //{
        //    ArrayList list = AsArrayList(property, "String");

        //    if (list.IndexOf(member) == -1) property += (list.CountForOwner == 0 ? "" : "\n") + member;
        //    return property;
        //}

        //public static string RemoveFromString(string property, string member)
        //{
        //    ArrayList list = AsArrayList(property, "String");
        //    if (list.IndexOf(property) == -1) list.Remove(property);
        //    return AsString(list, "String");
        //}

        //public static string AddToString(string property, int member)
        //{
        //    ArrayList list = AsArrayList(property, "String");

        //    if (list.IndexOf(member) == -1) property += (list.CountForOwner == 0 ? "" : "\n") + Convert.ToString(member);
        //    return property;
        //}

        //public static string RemoveFromString(string property, int member)
        //{
        //    ArrayList list = AsArrayList(property, "String");
        //    if (list.IndexOf(Convert.ToString(member)) == -1) list.Remove(Convert.ToString(member));
        //    return AsString(list, "String");
        //}

        public static string[] SplitOnNewLine(string s)
        {
            // returns a string array - split on newline
            if (s == null) return new string[0];
            return s.Split(new String[] { "\n" }, StringSplitOptions.None);
        }

        public static string[] SplitOnTab(string s)
        {
            // returns a string Array - string split on tab
            if (s == null) return new string[0];
            return s.Split(new String[] { "\t" }, StringSplitOptions.None);
        }

        public static Dictionary<string, string> Add(Dictionary<string, string> original, Dictionary<string, string> toBeAdded)
        {
            // Adds a dictionary to a dictionary
            foreach (var newValue in toBeAdded)
            {
                try
                {
                    original.Add(newValue.Key, newValue.Value);
                }
                catch
                {
                    // Do Nothing 
                }
            }
            return original;
        }

        public static Dictionary<string, int> Add(Dictionary<string, int> original, Dictionary<string, int> toBeAdded)
        {
            foreach (var newValue in toBeAdded)
            {
                try
                {
                    original.Add(newValue.Key, newValue.Value);
                }
                catch
                {
                    // Do Nothing 
                }
            }
            return original;
        }

        public static ArrayList AsArrayList(string s, string type = "Double", string[] delimiter = null)
        {
            if (String.IsNullOrEmpty(s)) return new ArrayList();

            if (delimiter == null) delimiter = new string[] { "\n", " ", ";", "\t" };

            // remove double spaces
            s = s.TrimEnd();
            s = s.TrimStart();
            while (s.Contains("  "))
            {
                s = s.Replace("  ", " ");
            }

            // makes a string array of all values
            string[] vals = s.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            // Reinitialize the ArrayList
            var arraylist = new ArrayList();
            int i = 0;
            foreach (string val in vals)
            {
                try
                {
                    switch (type)
                    {
                        case "Double":
                            arraylist.Add(Convert.ToDouble(val));
                            break;
                        case "Integer":
                            arraylist.Add(Convert.ToInt32(val));
                            break;
                        default:
                            arraylist.Add(val);
                            break;
                    }
                    i++;
                }
                catch
                { // Do nothing 
                }
            }
            return arraylist;
        }

        public static List<double> AsListDouble(string s)
        {
            var list = new List<double>();
            string[] rows = s.Split(new string[] { "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string row in rows)
            {
                list.Add(Convert.ToDouble(row));
            }
            return list;
        }

        public static List<int> AsListInteger(string s)
        {
            var list = new List<int>();
            if (String.IsNullOrEmpty(s)) return list;
            string[] rows = s.Split(new string[] { "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string row in rows)
            {
                list.Add(Convert.ToInt32(row));
            }
            return list;
        }

        public static List<string> AsListString(string s)
        {
            var list = new List<string>();
            string[] rows = s.Split(new string[] { "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string row in rows)
            {
                list.Add(row);
            }
            return list;
        }

        public static Dictionary<string, string> AsDictionary(string data)
        {
            if (String.IsNullOrEmpty(data)) return new Dictionary<string, string>();
            //values has first element as key and second element as value
            data = data.TrimEnd();
            data = data.TrimStart();
            while (data.Contains("  "))
            {
                data = data.Replace("  ", " ");
            }

            var dictionary = new Dictionary<string, string>();
            // makes a string array of all values
            string[] vals = data.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string v in vals)
            {
                string[] xy = v.Split(new String[] { "\n", " ", ";", ",", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                dictionary.Add(xy[0], xy[1]);
            }
            return dictionary;
        }

        public static DateTime AsDateTime(string time)
        {
            DateTime ret = DateTime.MinValue;
            try
            {
                ret = Convert.ToDateTime(time);
            }
            catch
            {
                ret = DateTime.MinValue;
            }
            return ret;
        }

        public string AsString(double value, int decimalPlaces)
        {
            string formatString = "{0:N" + decimalPlaces.ToString().Trim() + "}";
            return String.Format(formatString, value);
        }

        public static string AsString(ArrayList al, string type = "Double", int decimalPlaces = 0)
        {
            // Returns as string delimited by new line character
            if (al == null) return String.Empty;

            string s = String.Empty;
            string formatString = "{0:N" + decimalPlaces.ToString().Trim() + "}";

            foreach (var d in al)
            {
                switch (type)
                {
                    case "Double":
                        s += String.Format(formatString, (double)d) + "\n";
                        break;
                    default:
                        s += Convert.ToString(d) + "\n";
                        break;
                }
            }
            return s;
        }

        public static string AsString(Dictionary<string, string> values)
        {
            string s = String.Empty;
            foreach (KeyValuePair<string, string> pair in values)
            {
                s += pair.Key + "\t" + pair.Value + "\n";
            }
            return s.Trim();
        }

        public static string AsCleanString(string s, string type, int decimalPlaces = 0)
        {
            string[] vals = s.Split(new String[] { "\n", " ", ";", "\t" }, StringSplitOptions.RemoveEmptyEntries);

            // Reinitialize the ArrayList
            var arraylist = new ArrayList();
            int i = 0;
            foreach (string val in vals)
            {
                try
                {
                    switch (type)
                    {
                        case "Double":
                            arraylist.Add(Convert.ToDouble(val));
                            break;
                        case "Integer":
                            arraylist.Add(Convert.ToInt32(val));
                            break;
                        default:
                            arraylist.Add(val);
                            break;
                    }
                    i++;
                }
                catch
                { // Do nothing 
                }
            }
            // #Eaglin need to replace 5 with number of decimal places
            return AsString(arraylist, type, decimalPlaces);
        }

        public static string AsString(Dictionary<string, double> dict, int decimalPlaces = 0)
        {
            if (dict == null) return String.Empty;

            string s = String.Empty;
            string formatString = "{0:N" + decimalPlaces.ToString().Trim() + "}";

            foreach (var d in dict)
            {
                s += d.Key + "\t";
                s += String.Format(formatString, (d.Value)) + "\n";
            }
            return s;
        }


        public static string AsString(double[] d, int decimalPlaces)
        {
            string s = String.Empty;
            string formatString = "{0:N" + decimalPlaces.ToString().Trim() + "}";

            for (int i = 0; i < d.Length; i++)
            {
                s += String.Format(formatString, d[i]) + "\n";
            }
            return s;
        }

        public static string AsString(double[] a, double[] b, int decimalPlaces)
        {
            string s = String.Empty;
            string formatString = "{0:N" + decimalPlaces.ToString().Trim() + "}";

            for (int i = 0; i < a.Length; i++)
            {
                s += String.Format(formatString, a[i]);
                s += "\t";
                s += String.Format(formatString, b[i]);
                s += "\n";
            }
            return s;
        }

        public static string[] AsStringArray(string property, string delimiter = "\n")
        {
            if (property == null) return new string[] { }; // blank array
            string[] vals = property.Split(new String[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
            var s = new string[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                s[i] = Convert.ToString(vals[i]);
            }
            return s;
        }

        public static string[] AsStringArray(double[] values)
        {
            var s = new string[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                s[i] = Convert.ToString(values[i]);
            }
            return s;
        }


        public static object[] AsObjectArray(ArrayList al)
        {
            var objects = new object[al.Count];

            int i = 0;
            foreach (object o in al)
            {
                objects[i] = (double)o;
                i++;
            }
            return objects;
        }

        public static object[] AsObjectArray(string s)
        {
            ArrayList al = AsArrayList(s);
            return AsObjectArray(al);
        }

        public static double[] AsDoubleArray(ArrayList al)
        {
            var objects = new double[al.Count];

            int i = 0;
            foreach (object o in al)
            {
                objects[i] = (double)o;
                i++;
            }
            return objects;
        }

        public static double[,] AsDoubleMatrix(string s)
        {
            // Designed for column data of lookup tables
            // rows separated by \n  columns by \t

            ArrayList rows = AsArrayList(s, "String", new string[] { "\n" });
            int nRows = rows.Count;

            ArrayList cols = AsArrayList(rows[0].ToString(), "Double", new string[] { "\t" });
            int nCols = cols.Count;

            double[,] values = new double[nRows, nCols];

            for (int i = 0; i < nRows; i++)
            {
                ArrayList col = AsArrayList(rows[i].ToString(), "Double", new string[] { "\t" });
                for (int j = 0; j < nCols; j++)
                {
                    values[i, j] = Convert.ToDouble(col[j]);
                }
            }
            return values;
        }

        public static double[] AsDoubleArray(string s)
        {
            ArrayList al = AsArrayList(s);
            return AsDoubleArray(al);
        }

        public static string GetUserName(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                return context.User.Identity.Name;
            }
            else
            {
                return "Anonymous";
            }
        }

        public static string FormattedNumber(double number, int decimalPlaces)
        {
            string FormatString = "{0:N" + decimalPlaces.ToString().Trim() + "}";
            return String.Format(FormatString, number);
        }

        #endregion

    }


    public static class XmlObjectDatabase
    {
        // Allows for the storage and retrieval of objects from the database
        // That conform to IXmlPropertyObject 


        /*  
        CREATE TABLE XmlObject (
         id INT PRIMARY KEY IDENTITY(1,1),
         ParentId INT FOREIGN KEY REFERENCES XmlObject(id),
         ZOrder INT NULL,
         Class VARCHAR(255) NOT NULL,
         Name VARCHAR(255) NULL,
         Description VARCHAR(500) NULL,
         XMLData VARCHAR(MAX) NULL,
         OwnerUserID VARCHAR(255) NOT NULL,
         CreatedDate DATETIME NULL,
         LastEditedUserID VARCHAR(255) NULL,
         LastEditedDate DATETIME NULL,
         Version INT NULL)
        * * 
        */

        // Static Field Names in Database
        public static string id = "id";
        public static string Class = "Class";
        public static string Name = "Name";
        public static string Description = "Description";
        public static string XMLData = "XMLData";
        public static string OwnerUserId = "OwnerUserId";
        public static string Version = "Version";
        public static string LastEditedDate = "LastEditedDate";

        public static SqlConnection Connection()
        {
            // This should be read from the Web.config
            string cs = ConfigurationManager.ConnectionStrings["ObjectDatabase"].ConnectionString;
            return new SqlConnection(cs);
        }

        private static void SetDefaults(IXmlPropertyObject o)
        {
            if (o.Name == null) o.Name = "Name";
            if (o.Description == null) o.Description = "Description";
            if (o.OwnerUserId == null) o.OwnerUserId = "Owner";
        }

        #region Insert

        public static string Insert(IXmlPropertyObject o)
        {
            string objectName = o.ShortClassName();

            string sql = "INSERT INTO XmlObject ";
            sql += "(Class, Name, ParentId, ZOrder, Description, XMLData, OwnerUserID, Version, LastEditedDate)";
            sql += "VALUES";
            sql += "(@Class, @Name, @ParentId, @ZOrder, @Description, @XMLData, @OwnerUserID, 1, GETDATE())" +
                   " SELECT SCOPE_IDENTITY()";

            SetDefaults(o);

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Class", SqlDbType.VarChar);
                command.Parameters["@Class"].Value = o.ShortClassName();

                command.Parameters.Add("@Name", SqlDbType.VarChar);
                command.Parameters["@Name"].Value = o.Name;

                command.Parameters.Add("@ParentId", SqlDbType.Int);
                command.Parameters["@ParentId"].Value = o.ParentId;

                command.Parameters.Add("@ZOrder", SqlDbType.Int);
                command.Parameters["@ZOrder"].Value = o.ParentId;


                command.Parameters.Add("@Description", SqlDbType.VarChar);
                command.Parameters["@Description"].Value = o.Description;

                command.Parameters.Add("@XmlData", SqlDbType.VarChar);
                command.Parameters["@XmlData"].Value = o.XmlSerialize();

                command.Parameters.Add("@OwnerUserID", SqlDbType.VarChar);
                command.Parameters["@OwnerUserID"].Value = o.OwnerUserId;


                try
                {
                    connection.Open();
                    return Convert.ToString(command.ExecuteScalar());
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        public static string Import(string xmlData)
        {

            var doc = XDocument.Parse(xmlData);

            string ShortClassName = doc.Root.Name.ToString();
            string Name = doc.Root.Element("Name").Value;
            string Description = doc.Root.Element("Description").Value;
            string Owner = doc.Root.Element("Owner").Value;

            string sql = "INSERT INTO XmlObject ";
            sql += "(Class, Name, Description, XMLData, OwnerUserID, Version, LastEditedDate)";
            sql += "VALUES";
            sql += "(@Class, @Name, @Description, @XMLData, @OwnerUserID, 1, GETDATE())";


            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Class", SqlDbType.VarChar);
                command.Parameters["@Class"].Value = ShortClassName;

                command.Parameters.Add("@Name", SqlDbType.VarChar);
                command.Parameters["@Name"].Value = Name;

                command.Parameters.Add("@Description", SqlDbType.VarChar);
                command.Parameters["@Description"].Value = Description;

                command.Parameters.Add("@XmlData", SqlDbType.VarChar);
                command.Parameters["@XmlData"].Value = xmlData;

                command.Parameters.Add("@OwnerUserID", SqlDbType.VarChar);
                command.Parameters["@OwnerUserID"].Value = Owner;


                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    return "Successful Import";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        #endregion

        #region Select
        public static DataTable Select(int id)
        {
            string sql = "SELECT * FROM XmlObject ";
            sql += "WHERE id = @ID ";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@ID", SqlDbType.Int);
                command.Parameters["@ID"].Value = id;

                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return dt;
                }
                catch
                {
                    return new DataTable();
                }
            }
        }

        public static DataTable Select(string name)
        {
            string sql = "SELECT TOP 1 * FROM XmlObject ";
            sql += "WHERE Name = @Name ";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Name", SqlDbType.VarChar);
                command.Parameters["@Name"].Value = name;

                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return dt;
                }
                catch
                {
                    return new DataTable();
                }
            }
        }

        public static DataTable SelectAllForName(string name)
        {
            string sql = "SELECT * FROM XmlObject ";
            sql += "WHERE Name = @Name ORDER BY id DESC";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Name", SqlDbType.VarChar);
                command.Parameters["@Name"].Value = name;

                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return dt;
                }
                catch
                {
                    return new DataTable();
                }
            }
        }

        public static DataTable SelectAllForParentId(int parentId)
        {
            string sql = "SELECT * FROM XmlObject ";
            sql += "WHERE ParentId = @ParentId ORDER BY id DESC";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@ParentId", SqlDbType.Int);
                command.Parameters["@ParentId"].Value = parentId;

                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return dt;
                }
                catch
                {
                    return new DataTable();
                }
            }
        }

        public static DataTable SelectAllForNameAndClass(string name, string classname)
        {
            string sql = "SELECT * FROM XmlObject ";
            sql += "WHERE Name = @Name " +
                " AND Class = @Class " +
                   "ORDER BY id DESC";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Name", SqlDbType.VarChar);
                command.Parameters["@Name"].Value = name;

                command.Parameters.Add("@Class", SqlDbType.VarChar);
                command.Parameters["@Class"].Value = name;


                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return dt;
                }
                catch
                {
                    return new DataTable();
                }
            }
        }

        public static DataTable SelectAllChildrenInClass(int parentId, string className)
        {
            string sql = "SELECT * FROM XmlObject ";
            sql += "WHERE ParentId = @ParentId " +
                " AND Class = @Class " +
                   "ORDER BY id DESC";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@ParentId", SqlDbType.VarChar);
                command.Parameters["@ParentId"].Value = parentId;

                command.Parameters.Add("@Class", SqlDbType.VarChar);
                command.Parameters["@Class"].Value = className;


                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return dt;
                }
                catch
                {
                    return new DataTable();
                }
            }
        }


        public static DataTable SelectAllForName(string name, string owner)
        {
            string sql = "SELECT * FROM XmlObject ";
            sql += "WHERE Name = @Name ";
            sql += " AND OwnerUserId IN (@OwnerUserID, 'Global', 'Anonymous', 'Owner')";
            sql += " ORDER BY id DESC";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Name", SqlDbType.VarChar);
                command.Parameters["@Name"].Value = name;

                command.Parameters.Add("@OwnerUserID", SqlDbType.VarChar);
                command.Parameters["@OwnerUserID"].Value = owner;

                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return dt;
                }
                catch
                {
                    return new DataTable();
                }
            }
        }

        public static string SelectOwner(int id)
        {
            string sql = "SELECT OwnerUserId FROM XmlObject ";
            sql += "WHERE id = @ID ";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@ID", SqlDbType.Int);
                command.Parameters["@ID"].Value = id;

                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return Convert.ToString(dt.Rows[0][0]);
                }
                catch
                {
                    return String.Empty;
                }
            }
        }
        public static DataTable SelectAll(string objectClass)
        {
            string sql = "SELECT * FROM XmlObject ";
            sql += "WHERE Class = @Class ORDER BY LastEditedDate DESC";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Class", SqlDbType.VarChar);
                command.Parameters["@Class"].Value = objectClass;

                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return dt;
                }
                catch
                {
                    return new DataTable();
                }
            }
        }

        public static DataTable SearchName(string name, string objectClass)
        {
            string sql = "SELECT * FROM XmlObject ";
            sql += "WHERE Class = @Class ";
            sql += " AND Name Like '%'+ @Name + '%' ";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Class", SqlDbType.VarChar);
                command.Parameters["@Class"].Value = objectClass;

                command.Parameters.Add("@Name", SqlDbType.VarChar);
                command.Parameters["@Name"].Value = name;


                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return dt;
                }
                catch
                {
                    return new DataTable();
                }
            }

        }

        public static DataTable SelectHelp(string objectClass)
        {
            string sql = "SELECT * FROM XmlObject ";
            sql += "WHERE Name = @Name";
            sql += " AND Class='ObjectHelp'";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Name", SqlDbType.VarChar);
                command.Parameters["@Name"].Value = objectClass;
                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return dt;
                }
                catch
                {
                    return new DataTable();
                }
            }
        }

        public static DataTable SelectAll(string ownerId, string objectClass)
        {
            if (objectClass == String.Empty) return SelectAll(ownerId);

            string sql = "SELECT * FROM XmlObject ";
            sql += "WHERE Class = @Class";
            if (ownerId != String.Empty) sql += " AND OwnerUserId = @OwnerUserID ";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Class", SqlDbType.VarChar);
                command.Parameters["@Class"].Value = objectClass;

                if (ownerId != String.Empty)
                {
                    command.Parameters.Add("@OwnerUserID", SqlDbType.VarChar);
                    command.Parameters["@OwnerUserID"].Value = ownerId;
                }
                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return dt;
                }
                catch
                {
                    return new DataTable();
                }
            }
        }


        public static DataTable SelectAllForClass(string objectClass)
        {

            string sql = "SELECT * FROM XmlObject ";
            sql += "WHERE Class = @Class";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Class", SqlDbType.VarChar);
                command.Parameters["@Class"].Value = objectClass;

                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return dt;
                }
                catch
                {
                    return new DataTable();
                }
            }
        }

        public static DataTable SelectAllForClass(string objectClass, string owner)
        {

            string sql = "SELECT * FROM XmlObject ";
            sql += "WHERE Class = @Class AND " +
                   "OwnerUserID IN (@OwnerUserID, 'Anonymous', 'Global', 'Owner')";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Class", SqlDbType.VarChar);
                command.Parameters["@Class"].Value = objectClass;

                command.Parameters.Add("@OwnerUserID", SqlDbType.VarChar);
                command.Parameters["@OwnerUserID"].Value = owner;

                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return dt;
                }
                catch
                {
                    return new DataTable();
                }
            }
        }


        public static DataTable SelectAll(string[] owners, string objectClass)
        {

            string sql = "SELECT * FROM XmlObject ";
            sql += "WHERE Class = @Class";
            sql += " AND OwnerUserId IN ( ";

            var count = owners.Length;
            foreach (string s in owners)
            {
                sql += "'" + s + "'";
                if (--count > 0)
                    sql += ",";
                else
                    sql += ")";

            }

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Class", SqlDbType.VarChar);
                command.Parameters["@Class"].Value = objectClass;

                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return dt;
                }
                catch
                {
                    return new DataTable();
                }
            }
        }


        public static string SelectClass(int ID)
        {
            string sql = "SELECT Class FROM XmlObject ";
            sql += "WHERE id = @id";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@id", SqlDbType.VarChar);
                command.Parameters["@id"].Value = ID;


                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return Convert.ToString(dt.Rows[0][0]);
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        public static int SelectCount(string ownerID, string objectClass)
        {
            string sql = "SELECT COUNT(*) FROM XmlObject ";
            sql += "WHERE OwnerUserId = @OwnerUserID ";
            sql += "AND Class = @Class";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Class", SqlDbType.VarChar);
                command.Parameters["@Class"].Value = objectClass;


                command.Parameters.Add("@OwnerUserID", SqlDbType.VarChar);
                command.Parameters["@OwnerUserID"].Value = ownerID;

                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return Convert.ToInt32(dt.Rows[0][0]);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public static int GetMaxParentId(int ID)
        {
            string sql = "SELECT MAX(*) FROM XmlObject ";
            sql += "WHERE id = @ID ";
            SqlConnection connection = XmlObjectDatabase.Connection();

            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@id", SqlDbType.Int);
                command.Parameters["@ID"].Value = ID;

                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return Convert.ToInt32(dt.Rows[0][0]);
                }
                catch
                {
                    return 0;
                }
            }

        }

        public static int SelectCountForName(string name, string objectClass)
        {
            string sql = "SELECT COUNT(*) FROM XmlObject ";
            sql += "WHERE Name = @Name ";
            sql += "AND Class = @Class";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Class", SqlDbType.VarChar);
                command.Parameters["@Class"].Value = objectClass;


                command.Parameters.Add("@Name", SqlDbType.VarChar);
                command.Parameters["@Name"].Value = name;

                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return Convert.ToInt32(dt.Rows[0][0]);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public static DataTable SelectAllObjectClasses()
        {
            string sql = "SELECT DISTINCT(Class) FROM XmlObject ";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);


                try
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);
                    return dt;
                }
                catch
                {
                    return new DataTable();
                }
            }
        }

        #endregion

        #region Delete

        public static string Delete(int ID)
        {
            string sql = "DELETE FROM XmlObject ";
            sql += "WHERE id = @id ";


            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@id", SqlDbType.Int);
                command.Parameters["@id"].Value = ID;

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    return "Object Deleted";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        } // End Delete

        #endregion

        #region Update

        public static string Update(IXmlPropertyObject o)
        {
            string objectName = o.ShortClassName();

            string sql = "UPDATE XmlObject ";
            sql += "SET Class = @Class, ";
            sql += "Name = @Name, ";
            sql += "Description = @Description, ";
            sql += "XMLData = @XmlData, ";
            sql += "OwnerUserID = @OwnerUserID, ";
            sql += "LastEditedDate = GETDATE()";
            sql += "WHERE id = @id";

            SetDefaults(o);
            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Class", SqlDbType.VarChar);
                command.Parameters["@Class"].Value = o.ShortClassName();

                command.Parameters.Add("@Name", SqlDbType.VarChar);
                command.Parameters["@Name"].Value = o.Name;

                command.Parameters.Add("@Description", SqlDbType.VarChar);
                command.Parameters["@Description"].Value = o.Description;

                command.Parameters.Add("@XmlData", SqlDbType.VarChar);
                command.Parameters["@XmlData"].Value = o.XmlSerialize();

                command.Parameters.Add("@OwnerUserID", SqlDbType.VarChar);
                command.Parameters["@OwnerUserID"].Value = o.OwnerUserId;

                command.Parameters.Add("@id", SqlDbType.Int);
                command.Parameters["@id"].Value = o.Id;

                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    return "Data Updated";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        } // End Update

        #endregion

        public static string GlobalReplace(string field, string oldValue, string newValue)
        {
            string sql = "UPDATE XmlObject ";
            sql += "SET " + field + " = @NewValue";
            sql += " WHERE " + field + " = @OldValue";

            SqlConnection connection = XmlObjectDatabase.Connection();
            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@NewValue", SqlDbType.VarChar);
                command.Parameters["@NewValue"].Value = newValue;

                command.Parameters.Add("@OldValue", SqlDbType.VarChar);
                command.Parameters["@OldValue"].Value = oldValue;


                try
                {
                    connection.Open();
                    return Convert.ToString(command.ExecuteScalar());
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

    } // End Class

}