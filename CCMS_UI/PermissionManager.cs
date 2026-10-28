using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Web.SessionState;

namespace iJournalUI
{
    public static class PermissionManager
    {

        public static void ApplyPermissionToGridView(GridView grid, DataTable dtPermissionSet,int[] task)
        {
                        
            IEnumerator iterator = grid.Columns.GetEnumerator();
            bool bPermissionExist = false;
            int index = 0;
            int gridIndex = -1;
            //while (iterator.MoveNext() && index < task.Length)
            while (iterator.MoveNext())
            {
                //Locate the template column such as delete , edit ...
                gridIndex++;
                if (iterator.Current.ToString() == task[index].ToString ()) 
                {
                    bPermissionExist = false;
                    //check for permission in data table 
                    for (int counter = 0; counter < dtPermissionSet.Rows.Count; counter++)
                    {
                        if (dtPermissionSet.Rows[counter].ItemArray[1].ToString() == task[index].ToString ())
                        {
                            grid.Columns[gridIndex].HeaderText = null ;
                            bPermissionExist = true;
                            break;
                        }
                    }

                    // if the permission does not exists then delete the column from grid...
                    if (!bPermissionExist)
                    {
                        grid.Columns.Remove((DataControlField)iterator.Current);
                        
                        iterator = grid.Columns.GetEnumerator();
                        gridIndex = -1;
                    }                    
                    index++;
                    if (index == task.Length ) break;
                }
                
            }

        }

        public static string[,] BuildMappingForGrid(GridView grid)
        {
            string[,] gridMapping = new string[grid.Columns.Count,2];
            
            IEnumerator iterator = grid.Columns.GetEnumerator();
            bool bPermissionExist = false;
            int index = -1;
            int rows=0;
            int cols = 0;
            while (iterator.MoveNext())
            {
                index++;
                gridMapping[rows,cols] = iterator.Current.ToString();
                cols++;
                gridMapping[rows,cols] = index.ToString ();
                rows++;
                if (cols == 1) cols = 0;

            }
            return gridMapping;
           
        }
    }
}