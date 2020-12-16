using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ReportViewSetup
{
    public static class ControlTypeValueConverter
    {
       

        public static double? ConvertValueToDouble(object sourceValue)
        {
            if (sourceValue == null) return null;

            double outvalue;
            if (double.TryParse(sourceValue.ToString(), out outvalue))
            {
                return outvalue;
            }

            return null;
        }

        public static DateTime? ConvertValueToDate(object sourceValue)
        {
            if (sourceValue == null) return null;

            DateTime outvalue;
            if (DateTime.TryParse(sourceValue.ToString(), out outvalue))
            {
                return outvalue;
            }

            return null;
        }

        public static Boolean? ConvertValueToBoolean(object sourceValue)
        {
            if (sourceValue == null) return null;

            Boolean outvalue;
            if (Boolean.TryParse(sourceValue.ToString(), out outvalue))
            {
                return outvalue;
            }

            return null;
        }

        public static int? ConvertValueToInt(object sourceValue)
        {
            if (sourceValue == null) return null;

            int outvalue;
            if (int.TryParse(sourceValue.ToString(), out outvalue))
            {
                return outvalue;
            }

            return null;
        }

        public static Guid? ConvertValueToGuid(object sourceValue)
        {
            if (sourceValue == null) return null;

            Guid outvalue;
            if (Guid.TryParse(sourceValue.ToString(), out outvalue))
            {
                return outvalue;
            }

            return null;
        }


        public static decimal? ConvertValueToDecimal(object sourceValue)
        {
            if (sourceValue == null) return null;

            decimal outvalue;
            if (decimal.TryParse(sourceValue.ToString(), out outvalue))
            {
                return outvalue;
            }
            return null;
        }

        public static decimal? ConvertValueToDecimal(object sourceValue, int noofDeciaml)
        {
            if (sourceValue == null) return null;

            decimal outvalue;
            if (decimal.TryParse(sourceValue.ToString(), out outvalue))
            {
               outvalue = System.Math.Round(outvalue, noofDeciaml);
                return outvalue;
            }
            return null;
        }

        public static decimal ConvertValueToDecimalWithDefautZero(object sourceValue)
        {
            if (sourceValue == null) return 0;

            decimal outvalue;
            if (decimal.TryParse(sourceValue.ToString(), out outvalue))
            {
                return outvalue;
            }
            return 0;
        }

        public static string ConvertValueToStringWithDefaultEmptyString(object sourceValue)
        {
            if (sourceValue == null)
                return string.Empty;

            return sourceValue.ToString();
        }

        public static string ConvertDecimalToCultureFormat(object adoubleValue, CultureInfo aCultureInfo, int nbDecimal)
        {
            if (adoubleValue == null || string.IsNullOrEmpty(adoubleValue.ToString()))
                return string.Empty;

            try
            {
                if (nbDecimal == 0)
                {
                    return adoubleValue.ToString();
                }
                else
                {
                    double doubleNumber = Convert.ToDouble(adoubleValue.ToString());

                    string format = doubleNumber.ToString("N", aCultureInfo);

                    if (nbDecimal == 1)
                    {
                        format = doubleNumber.ToString("N1", aCultureInfo);
                    }
                    if (nbDecimal == 2)
                    {
                        format = doubleNumber.ToString("N2", aCultureInfo);
                    }
                    if (nbDecimal == 3)
                    {
                        format = doubleNumber.ToString("N3", aCultureInfo);
                    }
                    if (nbDecimal == 4)
                    {
                        format = doubleNumber.ToString("N4", aCultureInfo);
                    }

                    if (nbDecimal == 5)
                    {
                        format = doubleNumber.ToString("N5", aCultureInfo);
                    }

                    if (nbDecimal == 6)
                    {
                        format = doubleNumber.ToString("N6", aCultureInfo);
                    }

                    return format;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string ConvertCultureTextToDBMSDecimalFormat(int? nbDecimal, object valueText, CultureInfo aCultureInfo)
        {
            if (valueText == null || string.IsNullOrEmpty(valueText.ToString()))
                return string.Empty;
            try
            {
                decimal aValue = Convert.ToDecimal(valueText, aCultureInfo);
                if (nbDecimal.HasValue)
                {
                    string format = string.Format("{0:0}", aValue);

                    if (nbDecimal.Value == 1)
                    {
                        format = string.Format("{0:0.0}", aValue); // this will just cut the number off at the 3rd decimal place
                    }
                    if (nbDecimal.Value == 2)
                    {
                        format = string.Format("{0:0.00}", aValue);
                    }
                    if (nbDecimal.Value == 3)
                    {
                        format = string.Format("{0:0.000}", aValue);
                    }
                    if (nbDecimal.Value == 4)
                    {
                        format = string.Format("{0:0.0000}", aValue);
                    }

                    if (nbDecimal.Value == 5)
                    {
                        format = string.Format("{0:0.00000}", aValue);
                    }

                    if (nbDecimal.Value == 6)
                    {
                        format = string.Format("{0:0.000000}", aValue);
                    }

                    return format;
                }
                else
                {
                    return aValue.ToString();
                }
                
            }
            catch
            {
                return string.Empty;
            }
        }



        public static string GetStringFormatByNumberOfDecimal(int? nbDecimal)
        {
            string stringFormat = "0.00";

            if (nbDecimal.Value == 1)
            {
                stringFormat = "0.0";
            }
            if (nbDecimal.Value == 2)
            {
                stringFormat = "0.00";
            }
            if (nbDecimal.Value == 3)
            {
                stringFormat = "0.000";
            }
            if (nbDecimal.Value == 4)
            {
                stringFormat = "0.0000";
            }

            if (nbDecimal.Value == 5)
            {
                stringFormat = "0.00000";
            }         

            return stringFormat;
        }

        //public static string ConvertNumericStringFormatByCulture(int? nbDecimal, CultureInfo aCultureInfo)
        //{
        //    string enPrefix="{0:###,###,##0";
        //   // string frPrefix="{0:### ### ##0";
        //    string frPrefix = "{0:### ### ##0";

        //    string TrPrefix = "{0:###.###.##0";

        //    //'#,###.00##
        //    string format = "{0:0}";

        //    if (nbDecimal.HasValue)
        //        {
                  

        //            if (nbDecimal.Value == 1)
        //            {
        //                if( aCultureInfo.Name.StartsWith ("en"))
        //                {
        //                    format = enPrefix+".#}";                            
        //                }

        //                if( aCultureInfo.Name.StartsWith ("fr"))
        //                {
        //                    format = frPrefix + ".#}";
        //                }
                        
        //            }
        //            if (nbDecimal.Value == 2)
        //            {
        //                if( aCultureInfo.Name.StartsWith ("en"))
        //                {
        //                    format = enPrefix + ".##}";                            
        //                }

        //                if( aCultureInfo.Name.StartsWith ("fr"))
        //                {
        //                    format = frPrefix + ".##}";
        //                }
        //            }
        //            if (nbDecimal.Value == 3)
        //            {
        //                if( aCultureInfo.Name.StartsWith ("en"))
        //                {
        //                    format = enPrefix + ".###}";                            
        //                }

        //                if( aCultureInfo.Name.StartsWith ("fr"))
        //                {
        //                    format = frPrefix + ".###}";
        //                }
        //            }
        //            if (nbDecimal.Value == 4)
        //            {
        //                if( aCultureInfo.Name.StartsWith ("en"))
        //                {
        //                    format = enPrefix + ".####}";                            
        //                }

        //                if( aCultureInfo.Name.StartsWith ("fr"))
        //                {
        //                    format = frPrefix + ".####}";
        //                }
        //            }

        //            if (nbDecimal.Value == 5)
        //            {
        //                if( aCultureInfo.Name.StartsWith ("en"))
        //                {
        //                    format = enPrefix + ".#####}";                            
        //                }

        //                if( aCultureInfo.Name.StartsWith ("fr"))
        //                {
        //                    format = frPrefix + ".#####}";
        //                }
        //            }

        //            if (nbDecimal.Value == 6)
        //            {
        //                if( aCultureInfo.Name.StartsWith ("en"))
        //                {
        //                    format = enPrefix + ".######}";                            
        //                }

        //                if( aCultureInfo.Name.StartsWith ("fr"))
        //                {
        //                    format = frPrefix + ".######}";
        //                }
        //            }

                   
        //        }
        //    return format;               
        //}


        public static bool IsObjectNaNOrInfinity(object value)
        {         

            if (value is double)
            {
                return double.IsNaN((double)value);
            }

            else if (value is float)
            {
                return float.IsNaN((float)value) || float.IsInfinity((float)value);
            }
                       

            return false;
        }


    }

    public static class InchCMUnitConvert
    {
        private static char minusSign = '-';
        public static int nbDecimal = 4;

        public static string NotNumber = "NaN";

        private static decimal cmToInch(decimal dec)
        {
            double d = decimal.ToDouble(dec) / INCH_CM;
            return Convert.ToDecimal(d);
        }

        public static string DoubleToInch(double aDouble)
        {
            return DecimalToInch(Convert.ToDecimal(aDouble));
        }

        public static string DecimalStringToInch(string decimalCMstring)
        {
            if (string.IsNullOrEmpty(decimalCMstring))
                return "0";
            decimal outDecial = 0;
            if (decimal.TryParse(decimalCMstring, out outDecial))
            {
                return DecimalToInch(outDecial);
            }
            return "0";
        }

        public static string DecimalToInch(decimal cm)
        {
            if ((double)cm == 0.75)
            {
                
            }

            decimal orgCm = cm;

            cm = Math.Abs(cm);

            string toReturn = "0";

            decimal dec1 = cmToInch(cm);

            if (dec1 != 0)
            {
                if (dec1.ToString(CultureInfo.InvariantCulture).IndexOf('.') > 0)
                {
                    string strinch = dec1.ToString(CultureInfo.InvariantCulture);
                    //  string[] str = strinch.Split(".".ToCharArray(), 2);

                    string[] str = strinch.Split(".".ToCharArray());

                    double d = Convert.ToDouble("0." + str[1], CultureInfo.InvariantCulture);

                    int inch = (int)Math.Round((double)d / ONE_OF_SIXTYFOUR_INCH);
                    //					int inch = (int)Math.Round( (double) d / ONE_OF_THIRTYTWO_INCH );

                    if (str[0] != "0")
                    {
                        if (inch != 0)
                        {
                            if (inch == 64) // 32)
                            {
                                int approx = int.Parse(str[0]) + 1;
                                toReturn = approx.ToString();
                            }
                            else
                                toReturn = str[0] + Inchs(inch);
                        }
                        else
                            toReturn = str[0];
                    }
                    else
                    {
                        if (inch == 64) // 32)
                            return "1";
                        else
                            toReturn = Inchs(inch);
                    }
                }
                else
                    toReturn = dec1.ToString(CultureInfo.InvariantCulture);
            }

            if (orgCm < 0)
            {
                toReturn = "-" + toReturn;
            }
            return toReturn;
        }

        public static double InchToDouble(string inch)
        {
            return Convert.ToDouble(InchToDecimal(inch));
        }

        public static decimal CMToDecimal(string cm)
        {
            Decimal output = 0;

            try
            {
                output = Decimal.Parse(cm);
            }
            catch
            {
            }

            return output;
        }

        public static decimal InchToDecimal(string inch)
        {
            if (String.IsNullOrEmpty(inch))
                return 0;

            decimal returnValue = 0;

            string orgInch = inch;

            if (orgInch.IndexOf(minusSign) != -1)
            {
                inch = inch.Substring(orgInch.IndexOf(minusSign) + 1);
            }

            if (inch.IndexOf('/') > 0)
            {
                try
                {
                    double db;
                    string[] strInch = inch.Trim().Split(" ".ToCharArray());
                    if (strInch.Length > 1)
                    {
                        string[] rem = strInch[1].Trim().Split("/".ToCharArray());
                        db = double.Parse(strInch[0]) + (double.Parse(rem[0]) / double.Parse(rem[1]));
                    }
                    else
                    {
                        string[] rem = inch.Trim().Split("/".ToCharArray());
                        db = (double.Parse(rem[0]) / double.Parse(rem[1]));
                    }
                    returnValue = inchToCm(Convert.ToDecimal(db));
                }
                catch
                {
                    // return 0;
                }
            }
            else
            {
                try
                {
                    returnValue = inchToCm(decimal.Parse(inch.Trim()));
                }
                catch
                {
                    //return 0;
                }
            }

            if (orgInch.IndexOf(minusSign) != -1)
            {
                // keep the minuse value
                returnValue = returnValue * (-1);
            }

            return Math.Round(returnValue, nbDecimal);
        }

        
        private static int DivRem(int a, int b, out int result)
        {
            int c = a / b;
            result = a - (b * c);
            return c;
        }

        private static string Inchs(int inch)
        {
            int index, rem;

            // SL does not support Math.DivRem
            // index = Math.DivRem(inch, 2, out rem) - 1;
            // Math.DivRem (

            index = DivRem(inch, 2, out rem) - 1;
            if (rem == 0 && 0 <= index && index < 31)
                return special64[index];
            else
                return (" " + inch.ToString() + "/64");
        }

        private static decimal inchToCm(decimal dec)
        {
            double d = decimal.ToDouble(dec) * INCH_CM;

            return Math.Round(Convert.ToDecimal(d), nbDecimal);
        }

        private static readonly double INCH_CM = 2.54;

        //private static readonly double ONE_OF_THIRTYTWO_INCH = 0.03125;
        private static readonly string[] special32 = { " 1/16", " 1/8",  " 3/16", " 1/4",  " 5/16",
                                                        " 3/8",  " 7/16", " 1/2",  " 9/16", " 5/8",
                                                        " 11/16"," 3/4",  " 13/16"," 7/8",  " 15/16" };

        private static readonly double ONE_OF_SIXTYFOUR_INCH = 0.015625;
        private static readonly string[] special64 = {  " 1/32", " 1/16", " 3/32",  " 1/8",
                                                        " 5/32", " 3/16", " 7/32",  " 1/4",
                                                        " 9/32", " 5/16", " 11/32", " 3/8",
                                                        " 13/32"," 7/16", " 15/32", " 1/2",
                                                        " 17/32"," 9/16", " 19/32", " 5/8",
                                                        " 21/32"," 11/16"," 23/32", " 3/4",
                                                        " 25/32"," 13/16"," 27/32", " 7/8",
                                                        " 29/32"," 15/16"," 31/32", };

        //
    }
}