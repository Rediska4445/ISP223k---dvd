using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace isp223k___dvd
{
    [SuppressUnmanagedCodeSecurity]
    public class Dwm
    {
        public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;

        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;

            public MARGINS(int LeftWidth, int RightWidth, int TopHeight, int BottomHeight)
            {
                leftWidth = LeftWidth;
                rightWidth = RightWidth;
                topHeight = TopHeight;
                bottomHeight = BottomHeight;
            }

            public void SheetOfGlass()
            {
                leftWidth = -1;
                rightWidth = -1;
                topHeight = -1;
                bottomHeight = -1;
            }
        }

        [Flags]
        public enum DWM_BB
        {
            Enable = 1,
            BlurRegion = 2,
            TransitionOnMaximized = 4
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwmwindowattribute
        public enum DWMWINDOWATTRIBUTE : uint
        {
            NCRenderingEnabled = 1,       //Get atttribute
            NCRenderingPolicy,            //Enable or disable non-client rendering
            TransitionsForceDisabled,
            AllowNCPaint,
            CaptionButtonBounds,          //Get atttribute
            NonClientRtlLayout,
            ForceIconicRepresentation,
            Flip3DPolicy,
            ExtendedFrameBounds,          //Get atttribute
            HasIconicBitmap,
            DisallowPeek,
            ExcludedFromPeek,
            Cloak,
            Cloaked,                      //Get atttribute. Returns a DWMCLOACKEDREASON
            FreezeRepresentation,
            PassiveUpdateMode,
            UseHostBackDropBrush,
            AccentPolicy = 19,            // Win 10 (undocumented)
            ImmersiveDarkMode = 20,       // Win 11 22000
            WindowCornerPreference = 33,  // Win 11 22000
            BorderColor,                  // Win 11 22000
            CaptionColor,                 // Win 11 22000
            TextColor,                    // Win 11 22000
            VisibleFrameBorderThickness,  // Win 11 22000
            SystemBackdropType            // Win 11 22621
        }

        public enum DWMNCRENDERINGPOLICY : uint
        {
            UseWindowStyle, // Enable/disable non-client rendering based on window style
            Disabled,       // Disabled non-client rendering; window style is ignored
            Enabled,        // Enabled non-client rendering; window style is ignored
        };

        public enum DWMACCENTSTATE
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [Flags]

        public enum ThumbProperties_dwFlags : uint
        {
            RectDestination = 0x00000001,
            RectSource = 0x00000002,
            Opacity = 0x00000004,
            Visible = 0x00000008,
            SourceClientAreaOnly = 0x00000010
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct AccentPolicy
        {
            public DWMACCENTSTATE AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;

            public AccentPolicy(DWMACCENTSTATE accentState, int accentFlags, int gradientColor, int animationId)
            {
                AccentState = accentState;
                AccentFlags = accentFlags;
                GradientColor = gradientColor;
                AnimationId = animationId;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_BLURBEHIND
        {
            public DWM_BB dwFlags;
            public int fEnable;
            public IntPtr hRgnBlur;
            public int fTransitionOnMaximized;

            public DWM_BLURBEHIND(bool enabled)
            {
                dwFlags = DWM_BB.Enable;
                fEnable = (enabled) ? 1 : 0;
                hRgnBlur = IntPtr.Zero;
                fTransitionOnMaximized = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WinCompositionAttrData
        {
            public DWMWINDOWATTRIBUTE Attribute;
            public IntPtr Data;  //Will point to an AccentPolicy struct, where Attribute will be DWMWINDOWATTRIBUTE.AccentPolicy
            public int SizeOfData;

            public WinCompositionAttrData(DWMWINDOWATTRIBUTE attribute, IntPtr data, int sizeOfData)
            {
                Attribute = attribute;
                Data = data;
                SizeOfData = sizeOfData;
            }
        }

        //https://msdn.microsoft.com/en-us/library/windows/desktop/aa969508(v=vs.85).aspx
        [DllImport("dwmapi.dll")]
        internal static extern int DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

        //https://msdn.microsoft.com/it-it/library/windows/desktop/aa969512(v=vs.85).aspx
        [DllImport("dwmapi.dll")]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/aa969524(v=vs.85).aspx
        [DllImport("dwmapi.dll")]
        internal static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);

        [DllImport("User32.dll", SetLastError = true)]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WinCompositionAttrData data);

        public static bool WindowSetAttribute(IntPtr hWnd, DWMWINDOWATTRIBUTE attribute, int attributeValue)
        {
            int result = DwmSetWindowAttribute(hWnd, attribute, ref attributeValue, sizeof(int));
            return (result == 0);
        }

        public static void Windows10EnableBlurBehind(IntPtr hWnd)
        {
            DWMNCRENDERINGPOLICY policy = DWMNCRENDERINGPOLICY.Enabled;
            WindowSetAttribute(hWnd, DWMWINDOWATTRIBUTE.NCRenderingPolicy, (int)policy);

            AccentPolicy accPolicy = new AccentPolicy()
            {
                AccentState = DWMACCENTSTATE.ACCENT_ENABLE_BLURBEHIND,
            };

            int accentSize = Marshal.SizeOf(accPolicy);
            IntPtr accentPtr = Marshal.AllocHGlobal(accentSize);
            Marshal.StructureToPtr(accPolicy, accentPtr, false);
            var data = new WinCompositionAttrData(DWMWINDOWATTRIBUTE.AccentPolicy, accentPtr, accentSize);

            SetWindowCompositionAttribute(hWnd, ref data);
            Marshal.FreeHGlobal(accentPtr);
        }

        public static bool WindowEnableBlurBehind(IntPtr hWnd)
        {
            DWMNCRENDERINGPOLICY policy = DWMNCRENDERINGPOLICY.Enabled;
            WindowSetAttribute(hWnd, DWMWINDOWATTRIBUTE.NCRenderingPolicy, (int)policy);

            DWM_BLURBEHIND dwm_BB = new DWM_BLURBEHIND(true);
            int result = DwmEnableBlurBehindWindow(hWnd, ref dwm_BB);
            return result == 0;
        }

        public static bool WindowBorderlessDropShadow(IntPtr hWnd, int shadowSize)
        {
            MARGINS margins = new MARGINS(0, shadowSize, 0, shadowSize);
            int result = DwmExtendFrameIntoClientArea(hWnd, ref margins);
            return result == 0;
        }
    }   //DWM
}
