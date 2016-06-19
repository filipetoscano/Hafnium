using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Hafnium.Engine.Excel
{
    public class Spreadsheet : IDisposable
    {
        [DllImport( "user32.dll" )]
        private static extern uint GetWindowThreadProcessId( IntPtr hWnd, out uint lpdwProcessId );

        private readonly Dictionary<string, Worksheet> _worksheets = new Dictionary<string, Worksheet>();
        private Application _excelApplication;
        private Workbook _workbook;
        private bool _autoCalculate;
        private bool _disposed = false;


        public bool AutoCalculate
        {
            get
            {
                return _autoCalculate;
            }

            set
            {
                _autoCalculate = value;
                if ( _excelApplication != null )
                    _excelApplication.Calculation =
                        _autoCalculate ? XlCalculation.xlCalculationAutomatic : XlCalculation.xlCalculationManual;
            }
        }


        public Spreadsheet()
        {
            OpenExcelApplication();
        }


        private void OpenExcelApplication()
        {
            _excelApplication = new Application { Visible = false };
            _excelApplication.ScreenUpdating = false;
            _excelApplication.DisplayAlerts = false;
        }


        private Worksheet GetSheet( string name )
        {
            Worksheet sheet;

            if ( _worksheets.TryGetValue( name, out sheet ) == true )
                return sheet;

            sheet = _workbook.Sheets[ name ];
            _worksheets.Add( name, sheet );

            return sheet;
        }


        private Range GetCell( string cellReference )
        {
            Tuple<string, int, int> address = ParseReference( cellReference );
            Worksheet sheet = GetSheet( address.Item1 );
            Range range = (Range) sheet.Cells[ address.Item2, address.Item3 ];

            return range;
        }


        private static Tuple<string, int, int> ParseReference( string cellReference )
        {
            string sheet;
            string columnName;
            int column;
            int row;

            int sheetLength = cellReference.IndexOf( '!' );

            if ( sheetLength > 0 )
            {
                sheet = cellReference.Substring( 0, sheetLength );
            }
            else
            {
                sheet = string.Empty;
            }

            ++sheetLength;

            int i = sheetLength;
            StringBuilder sb = new StringBuilder();

            for ( ; i < cellReference.Length
                    && !Char.IsDigit( cellReference[ i ] )
                 ; ++i )
            {
                sb.Append( cellReference[ i ] );
            }

            columnName = sb.ToString();
            sb.Clear();

            for ( ; i < cellReference.Length && Char.IsDigit( cellReference[ i ] )
                  ; ++i )
            {
                sb.Append( cellReference[ i ] );
            }

            row = Int32.Parse( sb.ToString() );
            column = GetColumnNumber( columnName );

            return new Tuple<string, int, int>( sheet, row, column );
        }


        private static int GetColumnNumber( string name )
        {
            int number = 0;
            int pow = 1;

            for ( int i = name.Length - 1; i >= 0; i-- )
            {
                number += (name[ i ] - 'A' + 1) * pow;
                pow *= 26;
            }

            return number;
        }


        public void SetValue( string cellReference, object value )
        {
            #region Validations

            if ( cellReference == null )
                throw new ArgumentNullException( nameof( cellReference ) );

            #endregion

            var cell = GetCell( cellReference );
            cell.Value2 = XlsType.FromValue( value );
        }


        public object GetValue( Type type, string cellReference )
        {
            #region Validations

            if ( type == null )
                throw new ArgumentNullException( nameof( type ) );

            if ( cellReference == null )
                throw new ArgumentNullException( nameof( cellReference ) );

            #endregion

            var cell = GetCell( cellReference );

            return XlsType.ToValue( type, cell.Value2 );
        }


        public void Calculate()
        {
            _excelApplication.Calculate();
        }


        public void Load( Stream stream )
        {
            #region Validations

            if ( stream == null )
                throw new ArgumentNullException( nameof( stream ) );

            #endregion

            if ( _workbook != null )
                throw new NotSupportedException( "Workbook already open." );


            /*
             * 
             */
            var tempFileName = Path.GetTempFileName();

            using ( FileStream fs = File.OpenWrite( tempFileName ) )
            {
                stream.Position = 0;
                stream.CopyTo( fs );
                stream.Close();
            }


            /*
             * 
             */
            _workbook = _excelApplication.Workbooks.Open( tempFileName );
            _workbook.EnableAutoRecover = false;
            _workbook.ForceFullCalculation = false;

            _excelApplication.Calculation =
                        _autoCalculate ?
                        XlCalculation.xlCalculationAutomatic :
                        XlCalculation.xlCalculationManual;
        }


        public void Load( string filename )
        {
            #region Validations

            if ( filename == null )
                throw new ArgumentNullException( nameof( filename ) );

            #endregion

            if ( _workbook != null )
                throw new NotSupportedException( "Workbook already open." );


            /*
             * 
             */
            var tempFileName = Path.GetTempFileName();
            File.Copy( filename, tempFileName, true );


            /*
             * 
             */
            _workbook = _excelApplication.Workbooks.Open( tempFileName );
            _workbook.EnableAutoRecover = false;
            _workbook.ForceFullCalculation = false;

            _excelApplication.Calculation =
                        _autoCalculate ?
                        XlCalculation.xlCalculationAutomatic :
                        XlCalculation.xlCalculationManual;
        }


        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }


        protected virtual void Dispose( bool disposing )
        {
            if ( _disposed == true )
                return;

            if ( disposing == true )
            {
                _excelApplication.Quit();

                //var hWnd = _excelApplication.Application.Hwnd;
                //TryKillProcessByMainWindowHwnd( hWnd );
            }

            _disposed = true;
        }


        private static void TryKillProcessByMainWindowHwnd( int hWnd )
        {
            uint processId;
            GetWindowThreadProcessId( (IntPtr) hWnd, out processId );

            if ( processId == 0 )
                return;

            try
            {
                Process.GetProcessById( (int) processId ).Kill();
            }
            catch ( ArgumentException )
            {
            }
            catch ( Win32Exception )
            {
            }
            catch ( NotSupportedException )
            {
            }
            catch ( InvalidOperationException )
            {
            }
        }
    }
}
