// Generated with <3 by Discord.Sdk.Derive

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Discord.Sdk
{
    /// <summary>
    ///  ActivityActionTypes represents the type of invite being sent to a user.
    /// </summary>
    /// <remarks>
    ///  There are essentially two types of invites:
    ///  1: A user with an existing activity party can invite another user to join that existing party
    ///  2: A user can request to join the existing activity party of another user
    ///
    ///  See https://discord.com/developers/docs/rich-presence/overview for more information.
    ///
    /// </remarks>
    public enum ActivityActionTypes
    {
        Invalid = 0,
        Join = 1,
        JoinRequest = 5,
    }

    /// <summary>
    ///  Allows your game to control the privacy of the party the user is in.
    /// </summary>
    public enum ActivityPartyPrivacy
    {
        Private = 0,
        Public = 1,
    }

    /// <summary>
    ///  Discord RichPresence supports multiple types of activities that a user can be doing.
    /// </summary>
    /// <remarks>
    ///  For the SDK, the only activity type that is really relevant is `Playing`.
    ///  The others are provided for completeness.
    ///
    ///  See https://discord.com/developers/docs/rich-presence/overview for more information.
    ///
    /// </remarks>
    public enum ActivityTypes
    {
        Playing = 0,
        Streaming = 1,
        Listening = 2,
        Watching = 3,
        CustomStatus = 4,
        Competing = 5,
        HangStatus = 6,
    }

    /// <summary>
    ///  Controls which Discord RichPresence field is displayed in the user's status.
    /// </summary>
    /// <remarks>
    ///  See https://discord.com/developers/docs/rich-presence/overview for more information.
    ///
    /// </remarks>
    public enum StatusDisplayTypes
    {
        Name = 0,
        State = 1,
        Details = 2,
    }

    /// <summary>
    ///  Represents the type of platforms that an activity invite can be accepted on.
    /// </summary>
    [Flags]
    public enum ActivityGamePlatforms
    {
        Desktop = 1,
        Xbox = 2,
        Samsung = 4,
        IOS = 8,
        Android = 16,
        Embedded = 32,
        PS4 = 64,
        PS5 = 128,
    }

    /// <summary>
    ///  Enum representing various types of errors the SDK returns.
    /// </summary>
    public enum ErrorType
    {
        None = 0,
        NetworkError = 1,
        HTTPError = 2,
        ClientNotReady = 3,
        Disabled = 4,
        ClientDestroyed = 5,
        ValidationError = 6,
        Aborted = 7,
        AuthorizationFailed = 8,
        RPCError = 9,
    }

    /// <summary>
    ///  Enum that represents the various HTTP status codes that can be returned.
    /// </summary>
    /// <remarks>
    ///  You can read more about these at: https://developer.mozilla.org/en-US/docs/Web/HTTP/Status
    ///  For convenience, we have defined a couple of enum values that are non-standard HTTP codes to
    ///  represent certain types of errors.
    ///
    /// </remarks>
    public enum HttpStatusCode
    {
        None = 0,
        Continue = 100,
        SwitchingProtocols = 101,
        Processing = 102,
        EarlyHints = 103,
        Ok = 200,
        Created = 201,
        Accepted = 202,
        NonAuthoritativeInfo = 203,
        NoContent = 204,
        ResetContent = 205,
        PartialContent = 206,
        MultiStatus = 207,
        AlreadyReported = 208,
        ImUsed = 209,
        MultipleChoices = 300,
        MovedPermanently = 301,
        Found = 302,
        SeeOther = 303,
        NotModified = 304,
        TemporaryRedirect = 307,
        PermanentRedirect = 308,
        BadRequest = 400,
        Unauthorized = 401,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        ProxyAuthRequired = 407,
        RequestTimeout = 408,
        Conflict = 409,
        Gone = 410,
        LengthRequired = 411,
        PreconditionFailed = 412,
        PayloadTooLarge = 413,
        UriTooLong = 414,
        UnsupportedMediaType = 415,
        RangeNotSatisfiable = 416,
        ExpectationFailed = 417,
        MisdirectedRequest = 421,
        UnprocessableEntity = 422,
        Locked = 423,
        FailedDependency = 424,
        TooEarly = 425,
        UpgradeRequired = 426,
        PreconditionRequired = 428,
        TooManyRequests = 429,
        RequestHeaderFieldsTooLarge = 431,
        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        HttpVersionNotSupported = 505,
        VariantAlsoNegotiates = 506,
        InsufficientStorage = 507,
        LoopDetected = 508,
        NotExtended = 510,
        NetworkAuthorizationRequired = 511,
    }

    /// <summary>
    ///  Represents the crypto method used to generate a code challenge.
    /// </summary>
    /// <remarks>
    ///  The only method used by the SDK is sha256.
    ///
    /// </remarks>
    public enum AuthenticationCodeChallengeMethod
    {
        S256 = 0,
    }

    /// <summary>
    ///  Represents the type of integration the app will be installed as.
    /// </summary>
    public enum IntegrationType
    {
        GuildInstall = 0,
        UserInstall = 1,
    }

    /// <summary>
    ///  Enum that represents the various channel types on Discord.
    /// </summary>
    /// <remarks>
    ///  For more information see: https://discord.com/developers/docs/resources/channel
    ///
    /// </remarks>
    public enum ChannelType
    {
        GuildText = 0,
        Dm = 1,
        GuildVoice = 2,
        GroupDm = 3,
        GuildCategory = 4,
        GuildNews = 5,
        GuildStore = 6,
        GuildNewsThread = 10,
        GuildPublicThread = 11,
        GuildPrivateThread = 12,
        GuildStageVoice = 13,
        GuildDirectory = 14,
        GuildForum = 15,
        GuildMedia = 16,
        Lobby = 17,
        EphemeralDm = 18,
    }

    /// <summary>
    ///  Represents the type of additional content contained in a message.
    /// </summary>
    public enum AdditionalContentType
    {
        Other = 0,
        Attachment = 1,
        Poll = 2,
        VoiceMessage = 3,
        Thread = 4,
        Embed = 5,
        Sticker = 6,
    }

    /// <summary>
    ///  The Discord Voice audio system to use.
    /// </summary>
    public enum AudioSystem
    {
        Standard = 0,
        Game = 1,
    }

    /// <summary>
    ///  Represents whether a voice call is using push to talk or auto voice detection
    /// </summary>
    public enum AudioModeType
    {
        MODE_UNINIT = 0,
        MODE_VAD = 1,
        MODE_PTT = 2,
    }

    /// <summary>
    ///  Enum that represents the possible types of relationships that can exist between two users
    /// </summary>
    public enum RelationshipType
    {
        None = 0,
        Friend = 1,
        Blocked = 2,
        PendingIncoming = 3,
        PendingOutgoing = 4,
        Implicit = 5,
        Suggestion = 6,
    }

    /// <summary>
    ///  The type of external identity provider.
    /// </summary>
    public enum ExternalIdentityProviderType
    {
        OIDC = 0,
        EpicOnlineServices = 1,
        Steam = 2,
        Unity = 3,
        DiscordBot = 4,
        None = 5,
        Unknown = 6,
    }

    /// <summary>
    ///  Enum that specifies the various online statuses for a user.
    /// </summary>
    /// <remarks>
    ///  Generally a user is online or offline, but in Discord users are able to further customize their
    ///  status such as turning on "Do not Disturb" mode or "Dnd" to silence notifications.
    ///
    /// </remarks>
    public enum StatusType
    {
        Online = 0,
        Offline = 1,
        Blocked = 2,
        Idle = 3,
        Dnd = 4,
        Invisible = 5,
        Streaming = 6,
        Unknown = 7,
    }

    /// <summary>
    ///  Enum that represents various informational disclosures that Discord may make to users, so that
    ///  the game can identity them and customize their rendering as desired.
    /// </summary>
    /// <remarks>
    ///  See MessageHandle for more details.
    ///
    /// </remarks>
    public enum DisclosureTypes
    {
        MessageDataVisibleOnDiscord = 3,
    }

    /// <summary>
    ///  A bitfield that represents the various flags that can be set on a lobby member.
    /// </summary>
    public enum LobbyMemberFlags
    {
        None = 0,
        CanLinkLobby = 1,
    }

    /// <summary>
    ///  Represents the type of auth token used by the SDK, either the normal tokens produced by the
    ///  Discord desktop app, or an oauth2 bearer token. Only the latter can be used by the SDK.
    /// </summary>
    public enum AuthorizationTokenType
    {
        User = 0,
        Bearer = 1,
    }

    /// <summary>
    ///  Represents the various identity providers that can be used to authenticate a provisional
    ///  account user for public clients.
    /// </summary>
    public enum AuthenticationExternalAuthType
    {
        OIDC = 0,
        EpicOnlineServicesAccessToken = 1,
        EpicOnlineServicesIdToken = 2,
        SteamSessionTicket = 3,
        UnityServicesIdToken = 4,
        DiscordBotIssuedAccessToken = 5,
        AppleIdToken = 6,
        PlayStationNetworkIdToken = 7,
    }

    /// <summary>
    ///  Enum that represents the various log levels supported by the SDK.
    /// </summary>
    public enum LoggingSeverity
    {
        Verbose = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        None = 5,
    }

    /// <summary>
    ///  Enum that represents the logical groups of relationships based on online status and game
    ///  activity
    /// </summary>
    public enum RelationshipGroupType
    {
        OnlinePlayingGame = 0,
        OnlineElsewhere = 1,
        Offline = 2,
    }

    public static unsafe class NativeMethods
    {
#if UNITY_IOS && !UNITY_EDITOR
    public const string LibraryName = "__Internal";
#else
        public const string LibraryName = "discord_partner_sdk";
#endif

        static NativeMethods()
        {
            // It's possible that the scripting domain was unloaded while there
            // are still pending callbacks. Reset the queue just in case.
            Discord_ResetCallbacks();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void __Init()
        {
            // The real logic is in the static constructor.
        }

        public static void __ReportUnhandledException(Exception ex)
        {
            var handler = UnhandledException;

            if (handler != null)
            {
                handler(ex);
            }
            else
            {
                // Debug.LogException(ex);
            }
        }

#if UNITY_IOS && !UNITY_EDITOR
    public static void __OnPostConstruct(Discord.Sdk.Client client) {
        // Unity always activates the audio session on startup, tell the Discord voice engine
        // about this so that it doesn't deactivate it unexpectedly.
        client.SetEngineManagedAudioSession(true);
    }
#endif

        public static void __OnPostConstruct(object obj) { }

        public delegate void Discord_FreeFn(void* ptr);

        internal class ManagedUserData
        {
            public Delegate managedCallback;

            public static void* Free;
            private static readonly Discord_FreeFn _permanentlyRootedFreeDelegate = UnmanagedFree;

            public ManagedUserData(Delegate managedCallback) { this.managedCallback = managedCallback; }

            static ManagedUserData()
            {
                Free = (void*)Marshal.GetFunctionPointerForDelegate<Discord_FreeFn>(_permanentlyRootedFreeDelegate);
            }

            public static void UnmanagedFree(void* userData)
            {
                var handle = GCHandle.FromIntPtr((IntPtr)userData);
                handle.Free();
            }

            public static T DelegateFromPointer<T>(void* userData)
                where T : Delegate
            {
                var handle = GCHandle.FromIntPtr((IntPtr)userData);
                var userDataObj = (ManagedUserData)handle.Target!;
                return (T)userDataObj.managedCallback;
            }

            public static void* CreateHandle(Delegate cb)
            {
                var userData = new ManagedUserData(cb);
                return GCHandle.ToIntPtr(GCHandle.Alloc(userData)).ToPointer();
            }
        }

        public static event Action<Exception>? UnhandledException;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void __InitString(Discord_String* str, string value)
        {
            str->ptr = (byte*)Marshal.StringToCoTaskMemUTF8(value);
            str->size = (UIntPtr)Encoding.UTF8.GetByteCount(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void __FreeString(Discord_String* str)
        {
            Marshal.FreeCoTaskMem((IntPtr)str->ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static bool __InitStringLocal(byte* buf,
                                                    int* bufUsed,
                                                    int bufCapacity,
                                                    Discord_String* str,
                                                    string value)
        {
            var byteCount = Encoding.UTF8.GetByteCount(value);
            var alignedSize = (byteCount + 7) & ~7;

            if (*bufUsed + alignedSize > bufCapacity)
            {
                str->ptr = (byte*)Marshal.StringToCoTaskMemUTF8(value);
                str->size = (UIntPtr)byteCount;
                return true;
            }

            var span = new Span<byte>(buf + *bufUsed, bufCapacity - *bufUsed);
            var byteCountWritten = Encoding.UTF8.GetBytes(value, span);
            str->ptr = buf + *bufUsed;
            System.Diagnostics.Debug.Assert(byteCountWritten == byteCount);
            *bufUsed += alignedSize;
            str->size = (UIntPtr)byteCount;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool __InitNullableStringLocal(byte* buf,
                                                            int* bufUsed,
                                                            int bufCapacity,
                                                            Discord_String* str,
                                                            string? value)
        {
            if (value == null)
            {
                str->ptr = null;
                str->size = UIntPtr.Zero;
                return false;
            }

            return __InitStringLocal(buf, bufUsed, bufCapacity, str, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static bool __AllocLocal(byte* buf,
                                               int* bufUsed,
                                               int bufCapacity,
                                               void** ptrOut,
                                               int size)
        {
            var alignedSize = (size + 7) & ~7;

            if (*bufUsed + alignedSize > bufCapacity)
            {
                *ptrOut = (void*)Marshal.AllocCoTaskMem(size);
                return true;
            }

            *ptrOut = buf + *bufUsed + (alignedSize - size);
            *bufUsed += alignedSize;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static bool __AllocLocalStringArray(byte* buf,
                                                          int* bufUsed,
                                                          int bufCapacity,
                                                          Discord_String** ptrOut,
                                                          int count)
        {
            void* ptr;
            var owned = __AllocLocal(
                buf, bufUsed, bufCapacity, &ptr, count * sizeof(Discord_String));
            *ptrOut = (Discord_String*)ptr;
            return owned;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static bool __AllocateLocalBoolArray(byte* buf,
                                                           int* bufUsed,
                                                           int bufCapacity,
                                                           bool** ptrOut,
                                                           int count)
        {
            void* ptr;
            var owned = __AllocLocal(buf, bufUsed, bufCapacity, &ptr, count * sizeof(bool));
            *ptrOut = (bool*)ptr;
            return owned;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void __FreeLocalString(Discord_String* str, bool owned)
        {
            if (owned)
            {
                Marshal.FreeCoTaskMem((IntPtr)str->ptr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void __FreeLocal(void* ptr, bool owned)
        {
            if (owned)
            {
                Marshal.FreeCoTaskMem((IntPtr)ptr);
            }
        }

        [DllImport(LibraryName,
            EntryPoint = "Discord_Alloc",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void* Discord_Alloc(UIntPtr size);

        [DllImport(LibraryName,
            EntryPoint = "Discord_Free",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void Discord_Free(void* ptr);

        [DllImport(LibraryName,
            EntryPoint = "Discord_FreeProperties",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void Discord_FreeProperties(Discord_Properties props);

        [DllImport(LibraryName,
            EntryPoint = "Discord_SetFreeThreaded",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void Discord_SetFreeThreaded();

        [DllImport(LibraryName,
            EntryPoint = "Discord_RunCallbacks",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void Discord_RunCallbacks();

        [DllImport(LibraryName,
            EntryPoint = "Discord_ResetCallbacks",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void Discord_ResetCallbacks();

        [StructLayout(LayoutKind.Sequential)]
        public struct Discord_String
        {
            public byte* ptr;
            public UIntPtr size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Discord_ActivityButtonSpan
        {
            public ActivityButton* ptr;
            public UIntPtr size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Discord_UInt64Span
        {
            public ulong* ptr;
            public UIntPtr size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Discord_UserApplicationProfileHandleSpan
        {
            public UserApplicationProfileHandle* ptr;
            public UIntPtr size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Discord_LobbyMemberHandleSpan
        {
            public LobbyMemberHandle* ptr;
            public UIntPtr size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Discord_CallSpan
        {
            public Call* ptr;
            public UIntPtr size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Discord_AudioDeviceSpan
        {
            public AudioDevice* ptr;
            public UIntPtr size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Discord_MessageHandleSpan
        {
            public MessageHandle* ptr;
            public UIntPtr size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Discord_UserMessageSummarySpan
        {
            public UserMessageSummary* ptr;
            public UIntPtr size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Discord_GuildChannelSpan
        {
            public GuildChannel* ptr;
            public UIntPtr size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Discord_GuildMinimalSpan
        {
            public GuildMinimal* ptr;
            public UIntPtr size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Discord_RelationshipHandleSpan
        {
            public RelationshipHandle* ptr;
            public UIntPtr size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Discord_UserHandleSpan
        {
            public UserHandle* ptr;
            public UIntPtr size;
        }

        public struct Discord_Properties
        {
            public IntPtr size;
            public Discord_String* keys;
            public Discord_String* values;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ActivityInvite
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_Init",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Init(ActivityInvite* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(ActivityInvite* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(ActivityInvite* self, ActivityInvite* rhs);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_SenderId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong SenderId(ActivityInvite* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_SetSenderId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetSenderId(ActivityInvite* self, ulong value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_ChannelId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ChannelId(ActivityInvite* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_SetChannelId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetChannelId(ActivityInvite* self, ulong value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_MessageId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong MessageId(ActivityInvite* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_SetMessageId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetMessageId(ActivityInvite* self, ulong value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_Type",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ActivityActionTypes Type(ActivityInvite* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_SetType",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetType(ActivityInvite* self,
                                              ActivityActionTypes value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_ApplicationId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ApplicationId(ActivityInvite* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_SetApplicationId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetApplicationId(ActivityInvite* self, ulong value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_ParentApplicationId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ParentApplicationId(ActivityInvite* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_SetParentApplicationId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetParentApplicationId(ActivityInvite* self, ulong value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_PartyId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void PartyId(ActivityInvite* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_SetPartyId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetPartyId(ActivityInvite* self, Discord_String value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_SessionId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SessionId(ActivityInvite* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_SetSessionId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetSessionId(ActivityInvite* self, Discord_String value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_IsValid",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool IsValid(ActivityInvite* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityInvite_SetIsValid",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetIsValid(ActivityInvite* self, bool value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ActivityAssets
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_Init",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Init(ActivityAssets* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(ActivityAssets* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(ActivityAssets* self, ActivityAssets* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_LargeImage",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool LargeImage(ActivityAssets* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_SetLargeImage",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetLargeImage(ActivityAssets* self, Discord_String* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_LargeText",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool LargeText(ActivityAssets* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_SetLargeText",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetLargeText(ActivityAssets* self, Discord_String* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_LargeUrl",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool LargeUrl(ActivityAssets* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_SetLargeUrl",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetLargeUrl(ActivityAssets* self, Discord_String* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_SmallImage",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool SmallImage(ActivityAssets* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_SetSmallImage",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetSmallImage(ActivityAssets* self, Discord_String* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_SmallText",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool SmallText(ActivityAssets* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_SetSmallText",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetSmallText(ActivityAssets* self, Discord_String* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_SmallUrl",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool SmallUrl(ActivityAssets* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_SetSmallUrl",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetSmallUrl(ActivityAssets* self, Discord_String* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_InviteCoverImage",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool InviteCoverImage(ActivityAssets* self,
                                                       Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityAssets_SetInviteCoverImage",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetInviteCoverImage(ActivityAssets* self, Discord_String* value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ActivityTimestamps
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityTimestamps_Init",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Init(ActivityTimestamps* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityTimestamps_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(ActivityTimestamps* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityTimestamps_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(ActivityTimestamps* self, ActivityTimestamps* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityTimestamps_Start",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong Start(ActivityTimestamps* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityTimestamps_SetStart",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetStart(ActivityTimestamps* self, ulong value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityTimestamps_End",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong End(ActivityTimestamps* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityTimestamps_SetEnd",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetEnd(ActivityTimestamps* self, ulong value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ActivityParty
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityParty_Init",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Init(ActivityParty* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityParty_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(ActivityParty* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityParty_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(ActivityParty* self, ActivityParty* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityParty_Id",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Id(ActivityParty* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityParty_SetId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetId(ActivityParty* self, Discord_String value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityParty_CurrentSize",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern int CurrentSize(ActivityParty* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityParty_SetCurrentSize",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetCurrentSize(ActivityParty* self, int value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityParty_MaxSize",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern int MaxSize(ActivityParty* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityParty_SetMaxSize",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetMaxSize(ActivityParty* self, int value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityParty_Privacy",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ActivityPartyPrivacy Privacy(ActivityParty* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityParty_SetPrivacy",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetPrivacy(ActivityParty* self,
                                                 ActivityPartyPrivacy value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ActivitySecrets
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivitySecrets_Init",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Init(ActivitySecrets* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivitySecrets_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(ActivitySecrets* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivitySecrets_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(ActivitySecrets* self, ActivitySecrets* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivitySecrets_Join",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Join(ActivitySecrets* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivitySecrets_SetJoin",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetJoin(ActivitySecrets* self, Discord_String value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ActivityButton
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityButton_Init",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Init(ActivityButton* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityButton_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(ActivityButton* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityButton_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(ActivityButton* self, ActivityButton* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityButton_Label",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Label(ActivityButton* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityButton_SetLabel",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetLabel(ActivityButton* self, Discord_String value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityButton_Url",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Url(ActivityButton* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ActivityButton_SetUrl",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetUrl(ActivityButton* self, Discord_String value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Activity
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_Init",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Init(Activity* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(Activity* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(Activity* self, Activity* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_AddButton",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void AddButton(Activity* self, ActivityButton* button);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_Equals",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Equals(Activity* self, Activity* other);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_GetButtons",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetButtons(Activity* self,
                                                 Discord_ActivityButtonSpan* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_Name",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Name(Activity* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_SetName",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetName(Activity* self, Discord_String value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_Type",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ActivityTypes Type(Activity* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_SetType",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetType(Activity* self, ActivityTypes value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_StatusDisplayType",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool StatusDisplayType(Activity* self,
                                                        StatusDisplayTypes* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_SetStatusDisplayType",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetStatusDisplayType(Activity* self,
                                                           StatusDisplayTypes* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_State",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool State(Activity* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_SetState",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetState(Activity* self, Discord_String* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_StateUrl",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool StateUrl(Activity* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_SetStateUrl",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetStateUrl(Activity* self, Discord_String* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_Details",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Details(Activity* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_SetDetails",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetDetails(Activity* self, Discord_String* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_DetailsUrl",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool DetailsUrl(Activity* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_SetDetailsUrl",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetDetailsUrl(Activity* self, Discord_String* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_ApplicationId",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool ApplicationId(Activity* self, ulong* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_SetApplicationId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetApplicationId(Activity* self, ulong* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_ParentApplicationId",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool ParentApplicationId(Activity* self, ulong* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_SetParentApplicationId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetParentApplicationId(Activity* self, ulong* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_Assets",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Assets(Activity* self, ActivityAssets* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_SetAssets",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetAssets(Activity* self, ActivityAssets* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_Timestamps",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Timestamps(Activity* self, ActivityTimestamps* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_SetTimestamps",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetTimestamps(Activity* self, ActivityTimestamps* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_Party",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Party(Activity* self, ActivityParty* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_SetParty",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetParty(Activity* self, ActivityParty* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_Secrets",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Secrets(Activity* self, ActivitySecrets* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_SetSecrets",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetSecrets(Activity* self, ActivitySecrets* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_SupportedPlatforms",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ActivityGamePlatforms SupportedPlatforms(Activity* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Activity_SetSupportedPlatforms",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetSupportedPlatforms(Activity* self,
                                                            ActivityGamePlatforms value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ClientResult
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(ClientResult* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(ClientResult* self, ClientResult* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_ToString",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void ToString(ClientResult* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_Type",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ErrorType Type(ClientResult* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_SetType",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetType(ClientResult* self, ErrorType value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_Error",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Error(ClientResult* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_SetError",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetError(ClientResult* self, Discord_String value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_ErrorCode",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern int ErrorCode(ClientResult* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_SetErrorCode",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetErrorCode(ClientResult* self, int value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_Status",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern HttpStatusCode Status(ClientResult* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_SetStatus",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetStatus(ClientResult* self, HttpStatusCode value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_ResponseBody",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void ResponseBody(ClientResult* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_SetResponseBody",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetResponseBody(ClientResult* self, Discord_String value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_Successful",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Successful(ClientResult* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_SetSuccessful",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetSuccessful(ClientResult* self, bool value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_Retryable",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Retryable(ClientResult* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_SetRetryable",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetRetryable(ClientResult* self, bool value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_RetryAfter",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern float RetryAfter(ClientResult* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientResult_SetRetryAfter",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetRetryAfter(ClientResult* self, float value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AuthorizationCodeChallenge
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationCodeChallenge_Init",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Init(AuthorizationCodeChallenge* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationCodeChallenge_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(AuthorizationCodeChallenge* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationCodeChallenge_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(AuthorizationCodeChallenge* self,
                                            AuthorizationCodeChallenge* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationCodeChallenge_Method",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern AuthenticationCodeChallengeMethod Method(
                AuthorizationCodeChallenge* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationCodeChallenge_SetMethod",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetMethod(AuthorizationCodeChallenge* self,
                                                AuthenticationCodeChallengeMethod value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationCodeChallenge_Challenge",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Challenge(AuthorizationCodeChallenge* self,
                                                Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationCodeChallenge_SetChallenge",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetChallenge(AuthorizationCodeChallenge* self,
                                                   Discord_String value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AuthorizationCodeVerifier
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationCodeVerifier_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(AuthorizationCodeVerifier* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationCodeVerifier_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(AuthorizationCodeVerifier* self,
                                            AuthorizationCodeVerifier* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationCodeVerifier_Challenge",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Challenge(AuthorizationCodeVerifier* self,
                                                AuthorizationCodeChallenge* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationCodeVerifier_SetChallenge",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetChallenge(AuthorizationCodeVerifier* self,
                                                   AuthorizationCodeChallenge* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationCodeVerifier_Verifier",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Verifier(AuthorizationCodeVerifier* self,
                                               Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationCodeVerifier_SetVerifier",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetVerifier(AuthorizationCodeVerifier* self,
                                                  Discord_String value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AuthorizationArgs
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_Init",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Init(AuthorizationArgs* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(AuthorizationArgs* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(AuthorizationArgs* self, AuthorizationArgs* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_ClientId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ClientId(AuthorizationArgs* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_SetClientId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetClientId(AuthorizationArgs* self, ulong value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_Scopes",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Scopes(AuthorizationArgs* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_SetScopes",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetScopes(AuthorizationArgs* self, Discord_String value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_State",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool State(AuthorizationArgs* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_SetState",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetState(AuthorizationArgs* self, Discord_String* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_Nonce",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Nonce(AuthorizationArgs* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_SetNonce",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetNonce(AuthorizationArgs* self, Discord_String* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_CodeChallenge",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool CodeChallenge(AuthorizationArgs* self,
                                                    AuthorizationCodeChallenge* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_SetCodeChallenge",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetCodeChallenge(AuthorizationArgs* self,
                                                       AuthorizationCodeChallenge* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_IntegrationType",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool IntegrationType(AuthorizationArgs* self,
                                                      IntegrationType* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_SetIntegrationType",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetIntegrationType(AuthorizationArgs* self,
                                                         IntegrationType* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_CustomSchemeParam",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool CustomSchemeParam(AuthorizationArgs* self,
                                                        Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AuthorizationArgs_SetCustomSchemeParam",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetCustomSchemeParam(AuthorizationArgs* self,
                                                           Discord_String* value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceAuthorizationArgs
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_DeviceAuthorizationArgs_Init",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Init(DeviceAuthorizationArgs* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_DeviceAuthorizationArgs_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(DeviceAuthorizationArgs* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_DeviceAuthorizationArgs_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(DeviceAuthorizationArgs* self,
                                            DeviceAuthorizationArgs* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_DeviceAuthorizationArgs_ClientId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ClientId(DeviceAuthorizationArgs* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_DeviceAuthorizationArgs_SetClientId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetClientId(DeviceAuthorizationArgs* self, ulong value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_DeviceAuthorizationArgs_Scopes",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Scopes(DeviceAuthorizationArgs* self,
                                             Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_DeviceAuthorizationArgs_SetScopes",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetScopes(DeviceAuthorizationArgs* self, Discord_String value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct VoiceStateHandle
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_VoiceStateHandle_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(VoiceStateHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_VoiceStateHandle_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(VoiceStateHandle* self, VoiceStateHandle* other);

            [DllImport(LibraryName,
                EntryPoint = "Discord_VoiceStateHandle_SelfDeaf",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool SelfDeaf(VoiceStateHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_VoiceStateHandle_SelfMute",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool SelfMute(VoiceStateHandle* self);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct VADThresholdSettings
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_VADThresholdSettings_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(VADThresholdSettings* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_VADThresholdSettings_VadThreshold",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern float VadThreshold(VADThresholdSettings* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_VADThresholdSettings_SetVadThreshold",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetVadThreshold(VADThresholdSettings* self, float value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_VADThresholdSettings_Automatic",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Automatic(VADThresholdSettings* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_VADThresholdSettings_SetAutomatic",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetAutomatic(VADThresholdSettings* self, bool value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Call
        {
            public IntPtr Opaque0;
            public IntPtr Opaque1;
            public IntPtr Opaque2;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void OnVoiceStateChanged(ulong userId, void* __userData);

            public static void OnVoiceStateChanged_Handler(ulong userId, void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Call.OnVoiceStateChanged>(__userData);

                try
                {
                    __callback(userId);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void OnParticipantChanged(ulong userId, bool added, void* __userData);

            public static void OnParticipantChanged_Handler(ulong userId,
                                                            bool added,
                                                            void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Call.OnParticipantChanged>(__userData);

                try
                {
                    __callback(userId, added);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void OnSpeakingStatusChanged(ulong userId,
                                                         bool isPlayingSound,
                                                         void* __userData);

            public static void OnSpeakingStatusChanged_Handler(ulong userId,
                                                               bool isPlayingSound,
                                                               void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Call.OnSpeakingStatusChanged>(__userData);

                try
                {
                    __callback(userId, isPlayingSound);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void OnStatusChanged(Discord.Sdk.Call.Status status,
                                                 Discord.Sdk.Call.Error error,
                                                 int errorDetail,
                                                 void* __userData);

            public static void OnStatusChanged_Handler(Discord.Sdk.Call.Status status,
                                                       Discord.Sdk.Call.Error error,
                                                       int errorDetail,
                                                       void* __userData)
            {
                var __callback =
                    ManagedUserData.DelegateFromPointer<Discord.Sdk.Call.OnStatusChanged>(
                        __userData);

                try
                {
                    __callback(status, error, errorDetail);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(Call* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(Call* self, Call* other);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_ErrorToString",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void ErrorToString(Discord.Sdk.Call.Error type,
                                                    Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_GetAudioMode",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern AudioModeType GetAudioMode(Call* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_GetChannelId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong GetChannelId(Call* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_GetGuildId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong GetGuildId(Call* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_GetLocalMute",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetLocalMute(Call* self, ulong userId);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_GetParticipants",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetParticipants(Call* self, Discord_UInt64Span* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_GetParticipantVolume",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern float GetParticipantVolume(Call* self, ulong userId);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_GetPTTActive",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetPTTActive(Call* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_GetPTTReleaseDelay",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern uint GetPTTReleaseDelay(Call* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_GetSelfDeaf",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetSelfDeaf(Call* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_GetSelfMute",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetSelfMute(Call* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_GetStatus",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern Discord.Sdk.Call.Status GetStatus(Call* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_GetVADThreshold",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetVADThreshold(Call* self, VADThresholdSettings* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_GetVoiceStateHandle",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetVoiceStateHandle(Call* self,
                                                          ulong userId,
                                                          VoiceStateHandle* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_SetAudioMode",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetAudioMode(Call* self, AudioModeType audioMode);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_SetLocalMute",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetLocalMute(Call* self, ulong userId, bool mute);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_SetOnVoiceStateChangedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetOnVoiceStateChangedCallback(
                Call* self,
                OnVoiceStateChanged cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_SetParticipantChangedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetParticipantChangedCallback(
                Call* self,
                OnParticipantChanged cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_SetParticipantVolume",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetParticipantVolume(Call* self, ulong userId, float volume);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_SetPTTActive",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetPTTActive(Call* self, bool active);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_SetPTTReleaseDelay",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetPTTReleaseDelay(Call* self, uint releaseDelayMs);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_SetSelfDeaf",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetSelfDeaf(Call* self, bool deaf);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_SetSelfMute",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetSelfMute(Call* self, bool mute);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_SetSpeakingStatusChangedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetSpeakingStatusChangedCallback(
                Call* self,
                OnSpeakingStatusChanged cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_SetStatusChangedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetStatusChangedCallback(
                Call* self,
                OnStatusChanged cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_SetVADThreshold",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetVADThreshold(Call* self, bool automatic, float threshold);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Call_StatusToString",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void StatusToString(Discord.Sdk.Call.Status type,
                                                     Discord_String* returnValue);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ChannelHandle
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_ChannelHandle_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(ChannelHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ChannelHandle_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(ChannelHandle* self, ChannelHandle* other);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ChannelHandle_GetCallInfoHandle",
                   CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool GetCallInfoHandle(ChannelHandle* self,
                                                    CallInfoHandle* returnValue);
#endif
            [DllImport(LibraryName,
                EntryPoint = "Discord_ChannelHandle_Id",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong Id(ChannelHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ChannelHandle_Name",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Name(ChannelHandle* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ChannelHandle_Recipients",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Recipients(ChannelHandle* self, Discord_UInt64Span* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ChannelHandle_Type",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ChannelType Type(ChannelHandle* self);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GuildMinimal
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildMinimal_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(GuildMinimal* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildMinimal_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(GuildMinimal* self, GuildMinimal* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildMinimal_Id",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong Id(GuildMinimal* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildMinimal_SetId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetId(GuildMinimal* self, ulong value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildMinimal_Name",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Name(GuildMinimal* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildMinimal_SetName",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetName(GuildMinimal* self, Discord_String value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GuildChannel
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(GuildChannel* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(GuildChannel* self, GuildChannel* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_Id",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong Id(GuildChannel* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_SetId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetId(GuildChannel* self, ulong value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_Name",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Name(GuildChannel* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_SetName",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetName(GuildChannel* self, Discord_String value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_Type",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ChannelType Type(GuildChannel* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_SetType",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetType(GuildChannel* self, ChannelType value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_Position",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern int Position(GuildChannel* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_SetPosition",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetPosition(GuildChannel* self, int value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_ParentId",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool ParentId(GuildChannel* self, ulong* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_SetParentId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetParentId(GuildChannel* self, ulong* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_IsLinkable",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool IsLinkable(GuildChannel* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_SetIsLinkable",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetIsLinkable(GuildChannel* self, bool value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_IsViewableAndWriteableByAllMembers",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool IsViewableAndWriteableByAllMembers(GuildChannel* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_SetIsViewableAndWriteableByAllMembers",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetIsViewableAndWriteableByAllMembers(GuildChannel* self,
                                                                            bool value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_LinkedLobby",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool LinkedLobby(GuildChannel* self, LinkedLobby* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_GuildChannel_SetLinkedLobby",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetLinkedLobby(GuildChannel* self, LinkedLobby* value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LinkedLobby
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_LinkedLobby_Init",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Init(LinkedLobby* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LinkedLobby_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(LinkedLobby* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LinkedLobby_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(LinkedLobby* self, LinkedLobby* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LinkedLobby_ApplicationId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ApplicationId(LinkedLobby* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LinkedLobby_SetApplicationId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetApplicationId(LinkedLobby* self, ulong value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LinkedLobby_LobbyId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong LobbyId(LinkedLobby* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LinkedLobby_SetLobbyId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetLobbyId(LinkedLobby* self, ulong value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LinkedChannel
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_LinkedChannel_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(LinkedChannel* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LinkedChannel_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(LinkedChannel* self, LinkedChannel* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LinkedChannel_Id",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong Id(LinkedChannel* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LinkedChannel_SetId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetId(LinkedChannel* self, ulong value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LinkedChannel_Name",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Name(LinkedChannel* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LinkedChannel_SetName",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetName(LinkedChannel* self, Discord_String value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LinkedChannel_GuildId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong GuildId(LinkedChannel* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LinkedChannel_SetGuildId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetGuildId(LinkedChannel* self, ulong value);
        }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    [StructLayout(LayoutKind.Sequential)]
    public struct GuildMemberHandle {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildMemberHandle_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(GuildMemberHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildMemberHandle_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(GuildMemberHandle* self, GuildMemberHandle* other);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildMemberHandle_Avatar",
                   CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool Avatar(GuildMemberHandle* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildMemberHandle_AvatarUrl",
                   CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool AvatarUrl(GuildMemberHandle* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildMemberHandle_Deaf",
                   CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool Deaf(GuildMemberHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildMemberHandle_IsGuest",
                   CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool IsGuest(GuildMemberHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildMemberHandle_Mute",
                   CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool Mute(GuildMemberHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildMemberHandle_NickName",
                   CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool NickName(GuildMemberHandle* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildMemberHandle_SelfDeaf",
                   CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool SelfDeaf(GuildMemberHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildMemberHandle_SelfMute",
                   CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool SelfMute(GuildMemberHandle* self);
    }
#endif
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    [StructLayout(LayoutKind.Sequential)]
    public struct GuildHandle {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildHandle_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(GuildHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildHandle_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(GuildHandle* self, GuildHandle* other);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildHandle_ChannelIds",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelIds(GuildHandle* self, Discord_UInt64Span* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildHandle_GetGuildMemberHandle",
                   CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool GetGuildMemberHandle(GuildHandle* self,
                                                       ulong userId,
                                                       GuildMemberHandle* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildHandle_GuildMemberIds",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GuildMemberIds(GuildHandle* self,
                                                 Discord_UInt64Span* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildHandle_IconUrl",
                   CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool IconUrl(GuildHandle* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildHandle_Id",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong Id(GuildHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildHandle_Name",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Name(GuildHandle* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildHandle_PremiumTier",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern uint PremiumTier(GuildHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildHandle_Unavailable",
                   CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool Unavailable(GuildHandle* self);
    }
#endif
        [StructLayout(LayoutKind.Sequential)]
        public struct RelationshipHandle
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_RelationshipHandle_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(RelationshipHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_RelationshipHandle_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(RelationshipHandle* self, RelationshipHandle* other);

            [DllImport(LibraryName,
                EntryPoint = "Discord_RelationshipHandle_DiscordRelationshipType",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern RelationshipType DiscordRelationshipType(
                RelationshipHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_RelationshipHandle_GameRelationshipType",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern RelationshipType GameRelationshipType(
                RelationshipHandle* self);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        [DllImport(LibraryName,
                   EntryPoint = "Discord_RelationshipHandle_HasPlayedGame",
                   CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool HasPlayedGame(RelationshipHandle* self);
#endif
            [DllImport(LibraryName,
                EntryPoint = "Discord_RelationshipHandle_Id",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong Id(RelationshipHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_RelationshipHandle_IsSpamRequest",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool IsSpamRequest(RelationshipHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_RelationshipHandle_User",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool User(RelationshipHandle* self, UserHandle* returnValue);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UserApplicationProfileHandle
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserApplicationProfileHandle_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(UserApplicationProfileHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserApplicationProfileHandle_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(UserApplicationProfileHandle* self,
                                            UserApplicationProfileHandle* other);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserApplicationProfileHandle_AvatarHash",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void AvatarHash(UserApplicationProfileHandle* self,
                                                 Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserApplicationProfileHandle_Metadata",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Metadata(UserApplicationProfileHandle* self,
                                               Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserApplicationProfileHandle_ProviderId",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool ProviderId(UserApplicationProfileHandle* self,
                                                 Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserApplicationProfileHandle_ProviderIssuedUserId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void ProviderIssuedUserId(UserApplicationProfileHandle* self,
                                                           Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserApplicationProfileHandle_ProviderType",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ExternalIdentityProviderType ProviderType(
                UserApplicationProfileHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserApplicationProfileHandle_Username",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Username(UserApplicationProfileHandle* self,
                                               Discord_String* returnValue);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UserHandle
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserHandle_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(UserHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserHandle_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(UserHandle* self, UserHandle* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserHandle_Avatar",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Avatar(UserHandle* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserHandle_AvatarTypeToString",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void AvatarTypeToString(Discord.Sdk.UserHandle.AvatarType type,
                                                         Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserHandle_AvatarUrl",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void AvatarUrl(UserHandle* self,
                                                Discord.Sdk.UserHandle.AvatarType animatedType,
                                                Discord.Sdk.UserHandle.AvatarType staticType,
                                                Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserHandle_DisplayName",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void DisplayName(UserHandle* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserHandle_GameActivity",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GameActivity(UserHandle* self, Activity* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserHandle_GlobalName",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GlobalName(UserHandle* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserHandle_Id",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong Id(UserHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserHandle_IsProvisional",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool IsProvisional(UserHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserHandle_Relationship",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Relationship(UserHandle* self, RelationshipHandle* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserHandle_Status",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern StatusType Status(UserHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserHandle_UserApplicationProfiles",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void UserApplicationProfiles(
                UserHandle* self,
                Discord_UserApplicationProfileHandleSpan* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserHandle_Username",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Username(UserHandle* self, Discord_String* returnValue);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LobbyMemberHandle
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_LobbyMemberHandle_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(LobbyMemberHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LobbyMemberHandle_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(LobbyMemberHandle* self, LobbyMemberHandle* other);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LobbyMemberHandle_CanLinkLobby",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool CanLinkLobby(LobbyMemberHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LobbyMemberHandle_Connected",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Connected(LobbyMemberHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LobbyMemberHandle_Id",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong Id(LobbyMemberHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LobbyMemberHandle_Metadata",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Metadata(
                LobbyMemberHandle* self,
                Discord_Properties* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LobbyMemberHandle_User",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool User(LobbyMemberHandle* self, UserHandle* returnValue);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LobbyHandle
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_LobbyHandle_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(LobbyHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LobbyHandle_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(LobbyHandle* self, LobbyHandle* other);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LobbyHandle_GetCallInfoHandle",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetCallInfoHandle(LobbyHandle* self, CallInfoHandle* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LobbyHandle_GetLobbyMemberHandle",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetLobbyMemberHandle(LobbyHandle* self,
                                                           ulong memberId,
                                                           LobbyMemberHandle* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LobbyHandle_Id",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong Id(LobbyHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LobbyHandle_LinkedChannel",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool LinkedChannel(LobbyHandle* self, LinkedChannel* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LobbyHandle_LobbyMemberIds",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void LobbyMemberIds(LobbyHandle* self,
                                                     Discord_UInt64Span* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LobbyHandle_LobbyMembers",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void LobbyMembers(LobbyHandle* self,
                                                   Discord_LobbyMemberHandleSpan* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_LobbyHandle_Metadata",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Metadata(
                LobbyHandle* self,
                Discord_Properties* returnValue);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AdditionalContent
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_AdditionalContent_Init",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Init(AdditionalContent* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AdditionalContent_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(AdditionalContent* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AdditionalContent_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(AdditionalContent* self, AdditionalContent* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AdditionalContent_Equals",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Equals(AdditionalContent* self, AdditionalContent* rhs);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AdditionalContent_TypeToString",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void TypeToString(AdditionalContentType type,
                                                   Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AdditionalContent_Type",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern AdditionalContentType Type(AdditionalContent* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AdditionalContent_SetType",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetType(AdditionalContent* self,
                                              AdditionalContentType value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AdditionalContent_Title",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Title(AdditionalContent* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AdditionalContent_SetTitle",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetTitle(AdditionalContent* self, Discord_String* value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AdditionalContent_Count",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern byte Count(AdditionalContent* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AdditionalContent_SetCount",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetCount(AdditionalContent* self, byte value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MessageHandle
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(MessageHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(MessageHandle* self, MessageHandle* other);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_AdditionalContent",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool AdditionalContent(MessageHandle* self,
                                                        AdditionalContent* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_ApplicationId",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool ApplicationId(MessageHandle* self, ulong* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_Author",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Author(MessageHandle* self, UserHandle* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_AuthorId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong AuthorId(MessageHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_Channel",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Channel(MessageHandle* self, ChannelHandle* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_ChannelId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ChannelId(MessageHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_Content",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Content(MessageHandle* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_DisclosureType",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool DisclosureType(MessageHandle* self,
                                                     DisclosureTypes* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_EditedTimestamp",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong EditedTimestamp(MessageHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_Id",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong Id(MessageHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_Lobby",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Lobby(MessageHandle* self, LobbyHandle* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_Metadata",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Metadata(
                MessageHandle* self,
                Discord_Properties* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_ModerationMetadata",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void ModerationMetadata(
                MessageHandle* self,
                Discord_Properties* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_RawContent",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void RawContent(MessageHandle* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_Recipient",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Recipient(MessageHandle* self, UserHandle* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_RecipientId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong RecipientId(MessageHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_SentFromGame",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool SentFromGame(MessageHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_MessageHandle_SentTimestamp",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong SentTimestamp(MessageHandle* self);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AudioDevice
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_AudioDevice_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(AudioDevice* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AudioDevice_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(AudioDevice* self, AudioDevice* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AudioDevice_Equals",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Equals(AudioDevice* self, AudioDevice* rhs);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AudioDevice_Id",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Id(AudioDevice* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AudioDevice_SetId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetId(AudioDevice* self, Discord_String value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AudioDevice_Name",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Name(AudioDevice* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AudioDevice_SetName",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetName(AudioDevice* self, Discord_String value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AudioDevice_IsDefault",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool IsDefault(AudioDevice* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_AudioDevice_SetIsDefault",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetIsDefault(AudioDevice* self, bool value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UserMessageSummary
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserMessageSummary_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(UserMessageSummary* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserMessageSummary_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(UserMessageSummary* self, UserMessageSummary* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserMessageSummary_LastMessageId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong LastMessageId(UserMessageSummary* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_UserMessageSummary_UserId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong UserId(UserMessageSummary* self);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ClientCreateOptions
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientCreateOptions_Init",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Init(ClientCreateOptions* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientCreateOptions_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(ClientCreateOptions* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientCreateOptions_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(ClientCreateOptions* self, ClientCreateOptions* arg0);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientCreateOptions_WebBase",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void WebBase(ClientCreateOptions* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientCreateOptions_SetWebBase",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetWebBase(ClientCreateOptions* self, Discord_String value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientCreateOptions_ApiBase",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void ApiBase(ClientCreateOptions* self, Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientCreateOptions_SetApiBase",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetApiBase(ClientCreateOptions* self, Discord_String value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientCreateOptions_ExperimentalAudioSystem",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern AudioSystem ExperimentalAudioSystem(
                ClientCreateOptions* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientCreateOptions_SetExperimentalAudioSystem",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetExperimentalAudioSystem(ClientCreateOptions* self,
                                                                 AudioSystem value);

            [DllImport(LibraryName,
                EntryPoint =
                    "Discord_ClientCreateOptions_ExperimentalAndroidPreventCommsForBluetooth",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool ExperimentalAndroidPreventCommsForBluetooth(
                ClientCreateOptions* self);

            [DllImport(LibraryName,
                EntryPoint =
                    "Discord_ClientCreateOptions_SetExperimentalAndroidPreventCommsForBluetooth",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetExperimentalAndroidPreventCommsForBluetooth(
                ClientCreateOptions* self,
                bool value);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientCreateOptions_CpuAffinityMask",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool CpuAffinityMask(ClientCreateOptions* self, ulong* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_ClientCreateOptions_SetCpuAffinityMask",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetCpuAffinityMask(ClientCreateOptions* self, ulong* value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Client
        {
            public IntPtr Handle;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void UpdateLobbyMemberCallback(ClientResult* result,
                                                           ulong userId,
                                                           ulong lobbyId,
                                                           void* __userData);

            public static void UpdateLobbyMemberCallback_Handler(ClientResult* result,
                                                                 ulong userId,
                                                                 ulong lobbyId,
                                                                 void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.UpdateLobbyMemberCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0), userId, lobbyId);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void LobbyActionCallback(ClientResult* result,
                                                     ulong lobbyId,
                                                     void* __userData);

            public static void LobbyActionCallback_Handler(ClientResult* result,
                                                           ulong lobbyId,
                                                           void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.LobbyActionCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0), lobbyId);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void PerformOnThreadWithStringCallback(Discord_String text,
                                                                   void* __userData);

            public static void PerformOnThreadWithStringCallback_Handler(Discord_String text,
                                                                         void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.PerformOnThreadWithStringCallback>(
                            __userData);

                try
                {
                    __callback(Marshal.PtrToStringUTF8((IntPtr)text.ptr, (int)text.size));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(text.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void UpdateUserApplicationProfileCallback(ClientResult* result,
                                                                      void* __userData);

            public static void UpdateUserApplicationProfileCallback_Handler(ClientResult* result,
                                                                            void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.UpdateUserApplicationProfileCallback>(
                            __userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void EndCallCallback(void* __userData);

            public static void EndCallCallback_Handler(void* __userData)
            {
                var __callback =
                    ManagedUserData.DelegateFromPointer<Discord.Sdk.Client.EndCallCallback>(
                        __userData);

                try
                {
                    __callback();
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void EndCallsCallback(void* __userData);

            public static void EndCallsCallback_Handler(void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.EndCallsCallback>(__userData);

                try
                {
                    __callback();
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void GetCurrentInputDeviceCallback(AudioDevice* device, void* __userData);

            public static void GetCurrentInputDeviceCallback_Handler(AudioDevice* device,
                                                                     void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.GetCurrentInputDeviceCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.AudioDevice(device));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void GetCurrentOutputDeviceCallback(AudioDevice* device, void* __userData);

            public static void GetCurrentOutputDeviceCallback_Handler(AudioDevice* device,
                                                                      void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.GetCurrentOutputDeviceCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.AudioDevice(device));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void GetInputDevicesCallback(Discord_AudioDeviceSpan devices,
                                                         void* __userData);

            public static void GetInputDevicesCallback_Handler(Discord_AudioDeviceSpan devices,
                                                               void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.GetInputDevicesCallback>(__userData);

                try
                {
                    __callback(new Span<AudioDevice>(devices.ptr, (int)devices.size)
                               .ToArray()
                               .Select(__native => new Discord.Sdk.AudioDevice(__native, 0))
                               .ToArray());
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(devices.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void GetOutputDevicesCallback(Discord_AudioDeviceSpan devices,
                                                          void* __userData);

            public static void GetOutputDevicesCallback_Handler(Discord_AudioDeviceSpan devices,
                                                                void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.GetOutputDevicesCallback>(__userData);

                try
                {
                    __callback(new Span<AudioDevice>(devices.ptr, (int)devices.size)
                               .ToArray()
                               .Select(__native => new Discord.Sdk.AudioDevice(__native, 0))
                               .ToArray());
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(devices.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void DeviceChangeCallback(Discord_AudioDeviceSpan inputDevices,
                                                      Discord_AudioDeviceSpan outputDevices,
                                                      void* __userData);

            public static void DeviceChangeCallback_Handler(Discord_AudioDeviceSpan inputDevices,
                                                            Discord_AudioDeviceSpan outputDevices,
                                                            void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.DeviceChangeCallback>(__userData);

                try
                {
                    __callback(
                        new Span<AudioDevice>(inputDevices.ptr, (int)inputDevices.size)
                            .ToArray()
                            .Select(__native => new Discord.Sdk.AudioDevice(__native, 0))
                            .ToArray(),
                        new Span<AudioDevice>(outputDevices.ptr, (int)outputDevices.size)
                            .ToArray()
                            .Select(__native => new Discord.Sdk.AudioDevice(__native, 0))
                            .ToArray());
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(inputDevices.ptr);
                    Discord_Free(outputDevices.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void SetInputDeviceCallback(ClientResult* result, void* __userData);

            public static void SetInputDeviceCallback_Handler(ClientResult* result, void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.SetInputDeviceCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void NoAudioInputCallback(bool inputDetected, void* __userData);

            public static void NoAudioInputCallback_Handler(bool inputDetected, void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.NoAudioInputCallback>(__userData);

                try
                {
                    __callback(inputDetected);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void SetOutputDeviceCallback(ClientResult* result, void* __userData);

            public static void SetOutputDeviceCallback_Handler(ClientResult* result, void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.SetOutputDeviceCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void VoiceParticipantChangedCallback(ulong lobbyId,
                                                                 ulong memberId,
                                                                 bool added,
                                                                 void* __userData);

            public static void VoiceParticipantChangedCallback_Handler(ulong lobbyId,
                                                                       ulong memberId,
                                                                       bool added,
                                                                       void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.VoiceParticipantChangedCallback>(
                            __userData);

                try
                {
                    __callback(lobbyId, memberId, added);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void UserAudioReceivedCallback(ulong userId,
                                                           short* data,
                                                           ulong samplesPerChannel,
                                                           int sampleRate,
                                                           ulong channels,
                                                           bool* outShouldMute,
                                                           void* __userData);

            public static void UserAudioReceivedCallback_Handler(ulong userId,
                                                                 short* data,
                                                                 ulong samplesPerChannel,
                                                                 int sampleRate,
                                                                 ulong channels,
                                                                 bool* outShouldMute,
                                                                 void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.UserAudioReceivedCallback>(__userData);

                try
                {
                    __callback(userId,
                        (IntPtr)data,
                        samplesPerChannel,
                        sampleRate,
                        channels,
                        ref *outShouldMute);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void UserAudioCapturedCallback(short* data,
                                                           ulong samplesPerChannel,
                                                           int sampleRate,
                                                           ulong channels,
                                                           void* __userData);

            public static void UserAudioCapturedCallback_Handler(short* data,
                                                                 ulong samplesPerChannel,
                                                                 int sampleRate,
                                                                 ulong channels,
                                                                 void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.UserAudioCapturedCallback>(__userData);

                try
                {
                    __callback((IntPtr)data, samplesPerChannel, sampleRate, channels);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void AuthorizationCallback(ClientResult* result,
                                                       Discord_String code,
                                                       Discord_String redirectUri,
                                                       void* __userData);

            public static void AuthorizationCallback_Handler(ClientResult* result,
                                                             Discord_String code,
                                                             Discord_String redirectUri,
                                                             void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.AuthorizationCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0),
                        Marshal.PtrToStringUTF8((IntPtr)code.ptr, (int)code.size),
                        Marshal.PtrToStringUTF8((IntPtr)redirectUri.ptr, (int)redirectUri.size));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(code.ptr);
                    Discord_Free(redirectUri.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void ExchangeChildTokenCallback(
                ClientResult* result,
                Discord_String accessToken,
                AuthorizationTokenType tokenType,
                int expiresIn,
                Discord_String scopes,
                void* __userData);

            public static void ExchangeChildTokenCallback_Handler(
                ClientResult* result,
                Discord_String accessToken,
                AuthorizationTokenType tokenType,
                int expiresIn,
                Discord_String scopes,
                void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.ExchangeChildTokenCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0),
                        Marshal.PtrToStringUTF8((IntPtr)accessToken.ptr, (int)accessToken.size),
                        tokenType,
                        expiresIn,
                        Marshal.PtrToStringUTF8((IntPtr)scopes.ptr, (int)scopes.size));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(accessToken.ptr);
                    Discord_Free(scopes.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void FetchCurrentUserCallback(ClientResult* result,
                                                          ulong id,
                                                          Discord_String name,
                                                          void* __userData);

            public static void FetchCurrentUserCallback_Handler(ClientResult* result,
                                                                ulong id,
                                                                Discord_String name,
                                                                void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.FetchCurrentUserCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0),
                        id,
                        Marshal.PtrToStringUTF8((IntPtr)name.ptr, (int)name.size));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(name.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void TokenExchangeCallback(ClientResult* result,
                                                       Discord_String accessToken,
                                                       Discord_String refreshToken,
                                                       AuthorizationTokenType tokenType,
                                                       int expiresIn,
                                                       Discord_String scopes,
                                                       void* __userData);

            public static void TokenExchangeCallback_Handler(
                ClientResult* result,
                Discord_String accessToken,
                Discord_String refreshToken,
                AuthorizationTokenType tokenType,
                int expiresIn,
                Discord_String scopes,
                void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.TokenExchangeCallback>(__userData);

                try
                {
                    __callback(
                        new Discord.Sdk.ClientResult(*result, 0),
                        Marshal.PtrToStringUTF8((IntPtr)accessToken.ptr, (int)accessToken.size),
                        Marshal.PtrToStringUTF8((IntPtr)refreshToken.ptr, (int)refreshToken.size),
                        tokenType,
                        expiresIn,
                        Marshal.PtrToStringUTF8((IntPtr)scopes.ptr, (int)scopes.size));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(accessToken.ptr);
                    Discord_Free(refreshToken.ptr);
                    Discord_Free(scopes.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void AuthorizeRequestCallback(void* __userData);

            public static void AuthorizeRequestCallback_Handler(void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.AuthorizeRequestCallback>(__userData);

                try
                {
                    __callback();
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RevokeTokenCallback(ClientResult* result, void* __userData);

            public static void RevokeTokenCallback_Handler(ClientResult* result, void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.RevokeTokenCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void AuthorizeDeviceScreenClosedCallback(void* __userData);

            public static void AuthorizeDeviceScreenClosedCallback_Handler(void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.AuthorizeDeviceScreenClosedCallback>(
                            __userData);

                try
                {
                    __callback();
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void TokenExpirationCallback(void* __userData);

            public static void TokenExpirationCallback_Handler(void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.TokenExpirationCallback>(__userData);

                try
                {
                    __callback();
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void UnmergeIntoProvisionalAccountCallback(ClientResult* result,
                                                                       void* __userData);

            public static void UnmergeIntoProvisionalAccountCallback_Handler(ClientResult* result,
                                                                             void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.UnmergeIntoProvisionalAccountCallback>(
                            __userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void UpdateProvisionalAccountDisplayNameCallback(ClientResult* result,
                                                                             void* __userData);

            public static void UpdateProvisionalAccountDisplayNameCallback_Handler(ClientResult* result,
                                                                                   void* __userData)
            {
                var __callback = ManagedUserData.DelegateFromPointer<
                    Discord.Sdk.Client.UpdateProvisionalAccountDisplayNameCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void UpdateTokenCallback(ClientResult* result, void* __userData);

            public static void UpdateTokenCallback_Handler(ClientResult* result, void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.UpdateTokenCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void DeleteUserMessageCallback(ClientResult* result, void* __userData);

            public static void DeleteUserMessageCallback_Handler(ClientResult* result,
                                                                 void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.DeleteUserMessageCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void EditUserMessageCallback(ClientResult* result, void* __userData);

            public static void EditUserMessageCallback_Handler(ClientResult* result, void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.EditUserMessageCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void GetLobbyMessagesCallback(ClientResult* result,
                                                          Discord_MessageHandleSpan messages,
                                                          void* __userData);

            public static void GetLobbyMessagesCallback_Handler(ClientResult* result,
                                                                Discord_MessageHandleSpan messages,
                                                                void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.GetLobbyMessagesCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0),
                        new Span<MessageHandle>(messages.ptr, (int)messages.size)
                            .ToArray()
                            .Select(__native => new Discord.Sdk.MessageHandle(__native, 0))
                            .ToArray());
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(messages.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void UserMessageSummariesCallback(ClientResult* result,
                                                              Discord_UserMessageSummarySpan summaries,
                                                              void* __userData);

            public static void UserMessageSummariesCallback_Handler(
                ClientResult* result,
                Discord_UserMessageSummarySpan summaries,
                void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.UserMessageSummariesCallback>(__userData);

                try
                {
                    __callback(
                        new Discord.Sdk.ClientResult(*result, 0),
                        new Span<UserMessageSummary>(summaries.ptr, (int)summaries.size)
                            .ToArray()
                            .Select(__native => new Discord.Sdk.UserMessageSummary(__native, 0))
                            .ToArray());
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(summaries.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void UserMessagesWithLimitCallback(ClientResult* result,
                                                               Discord_MessageHandleSpan messages,
                                                               void* __userData);

            public static void UserMessagesWithLimitCallback_Handler(ClientResult* result,
                                                                     Discord_MessageHandleSpan messages,
                                                                     void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.UserMessagesWithLimitCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0),
                        new Span<MessageHandle>(messages.ptr, (int)messages.size)
                            .ToArray()
                            .Select(__native => new Discord.Sdk.MessageHandle(__native, 0))
                            .ToArray());
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(messages.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void ProvisionalUserMergeRequiredCallback(void* __userData);

            public static void ProvisionalUserMergeRequiredCallback_Handler(void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.ProvisionalUserMergeRequiredCallback>(
                            __userData);

                try
                {
                    __callback();
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void OpenMessageInDiscordCallback(ClientResult* result, void* __userData);

            public static void OpenMessageInDiscordCallback_Handler(ClientResult* result,
                                                                    void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.OpenMessageInDiscordCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void SendUserMessageCallback(ClientResult* result,
                                                         ulong messageId,
                                                         void* __userData);

            public static void SendUserMessageCallback_Handler(ClientResult* result,
                                                               ulong messageId,
                                                               void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.SendUserMessageCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0), messageId);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void MessageCreatedCallback(ulong messageId, void* __userData);

            public static void MessageCreatedCallback_Handler(ulong messageId, void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.MessageCreatedCallback>(__userData);

                try
                {
                    __callback(messageId);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void MessageDeletedCallback(ulong messageId,
                                                        ulong channelId,
                                                        void* __userData);

            public static void MessageDeletedCallback_Handler(ulong messageId,
                                                              ulong channelId,
                                                              void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.MessageDeletedCallback>(__userData);

                try
                {
                    __callback(messageId, channelId);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void MessageUpdatedCallback(ulong messageId, void* __userData);

            public static void MessageUpdatedCallback_Handler(ulong messageId, void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.MessageUpdatedCallback>(__userData);

                try
                {
                    __callback(messageId);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void LogCallback(Discord_String message,
                                             LoggingSeverity severity,
                                             void* __userData);

            public static void LogCallback_Handler(Discord_String message,
                                                   LoggingSeverity severity,
                                                   void* __userData)
            {
                var __callback =
                    ManagedUserData.DelegateFromPointer<Discord.Sdk.Client.LogCallback>(
                        __userData);

                try
                {
                    __callback(Marshal.PtrToStringUTF8((IntPtr)message.ptr, (int)message.size),
                        severity);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(message.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void OpenConnectedGamesSettingsInDiscordCallback(ClientResult* result,
                                                                             void* __userData);

            public static void OpenConnectedGamesSettingsInDiscordCallback_Handler(ClientResult* result,
                                                                                   void* __userData)
            {
                var __callback = ManagedUserData.DelegateFromPointer<
                    Discord.Sdk.Client.OpenConnectedGamesSettingsInDiscordCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void OnStatusChanged(Discord.Sdk.Client.Status status,
                                                 Discord.Sdk.Client.Error error,
                                                 int errorDetail,
                                                 void* __userData);

            public static void OnStatusChanged_Handler(Discord.Sdk.Client.Status status,
                                                       Discord.Sdk.Client.Error error,
                                                       int errorDetail,
                                                       void* __userData)
            {
                var __callback =
                    ManagedUserData.DelegateFromPointer<Discord.Sdk.Client.OnStatusChanged>(
                        __userData);

                try
                {
                    __callback(status, error, errorDetail);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void CreateOrJoinLobbyCallback(ClientResult* result,
                                                           ulong lobbyId,
                                                           void* __userData);

            public static void CreateOrJoinLobbyCallback_Handler(ClientResult* result,
                                                                 ulong lobbyId,
                                                                 void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.CreateOrJoinLobbyCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0), lobbyId);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void GetGuildChannelsCallback(ClientResult* result,
                                                          Discord_GuildChannelSpan guildChannels,
                                                          void* __userData);

            public static void GetGuildChannelsCallback_Handler(ClientResult* result,
                                                                Discord_GuildChannelSpan guildChannels,
                                                                void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.GetGuildChannelsCallback>(__userData);

                try
                {
                    __callback(
                        new Discord.Sdk.ClientResult(*result, 0),
                        new Span<GuildChannel>(guildChannels.ptr, (int)guildChannels.size)
                            .ToArray()
                            .Select(__native => new Discord.Sdk.GuildChannel(__native, 0))
                            .ToArray());
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(guildChannels.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void GetUserGuildsCallback(ClientResult* result,
                                                       Discord_GuildMinimalSpan guilds,
                                                       void* __userData);

            public static void GetUserGuildsCallback_Handler(ClientResult* result,
                                                             Discord_GuildMinimalSpan guilds,
                                                             void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.GetUserGuildsCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0),
                        new Span<GuildMinimal>(guilds.ptr, (int)guilds.size)
                            .ToArray()
                            .Select(__native => new Discord.Sdk.GuildMinimal(__native, 0))
                            .ToArray());
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(guilds.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void JoinLinkedLobbyGuildCallback(ClientResult* result,
                                                              Discord_String inviteUrl,
                                                              void* __userData);

            public static void JoinLinkedLobbyGuildCallback_Handler(ClientResult* result,
                                                                    Discord_String inviteUrl,
                                                                    void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.JoinLinkedLobbyGuildCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0),
                        Marshal.PtrToStringUTF8((IntPtr)inviteUrl.ptr, (int)inviteUrl.size));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(inviteUrl.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void LeaveLobbyCallback(ClientResult* result, void* __userData);

            public static void LeaveLobbyCallback_Handler(ClientResult* result, void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.LeaveLobbyCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void LinkOrUnlinkChannelCallback(ClientResult* result, void* __userData);

            public static void LinkOrUnlinkChannelCallback_Handler(ClientResult* result,
                                                                   void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.LinkOrUnlinkChannelCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void LobbyCreatedCallback(ulong lobbyId, void* __userData);

            public static void LobbyCreatedCallback_Handler(ulong lobbyId, void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.LobbyCreatedCallback>(__userData);

                try
                {
                    __callback(lobbyId);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void LobbyDeletedCallback(ulong lobbyId, void* __userData);

            public static void LobbyDeletedCallback_Handler(ulong lobbyId, void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.LobbyDeletedCallback>(__userData);

                try
                {
                    __callback(lobbyId);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void LobbyMemberAddedCallback(ulong lobbyId,
                                                          ulong memberId,
                                                          void* __userData);

            public static void LobbyMemberAddedCallback_Handler(ulong lobbyId,
                                                                ulong memberId,
                                                                void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.LobbyMemberAddedCallback>(__userData);

                try
                {
                    __callback(lobbyId, memberId);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void LobbyMemberRemovedCallback(ulong lobbyId,
                                                            ulong memberId,
                                                            void* __userData);

            public static void LobbyMemberRemovedCallback_Handler(ulong lobbyId,
                                                                  ulong memberId,
                                                                  void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.LobbyMemberRemovedCallback>(__userData);

                try
                {
                    __callback(lobbyId, memberId);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void LobbyMemberUpdatedCallback(ulong lobbyId,
                                                            ulong memberId,
                                                            void* __userData);

            public static void LobbyMemberUpdatedCallback_Handler(ulong lobbyId,
                                                                  ulong memberId,
                                                                  void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.LobbyMemberUpdatedCallback>(__userData);

                try
                {
                    __callback(lobbyId, memberId);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void LobbyUpdatedCallback(ulong lobbyId, void* __userData);

            public static void LobbyUpdatedCallback_Handler(ulong lobbyId, void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.LobbyUpdatedCallback>(__userData);

                try
                {
                    __callback(lobbyId);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void IsDiscordAppInstalledCallback(bool installed, void* __userData);

            public static void IsDiscordAppInstalledCallback_Handler(bool installed, void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.IsDiscordAppInstalledCallback>(__userData);

                try
                {
                    __callback(installed);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void AcceptActivityInviteCallback(ClientResult* result,
                                                              Discord_String joinSecret,
                                                              void* __userData);

            public static void AcceptActivityInviteCallback_Handler(ClientResult* result,
                                                                    Discord_String joinSecret,
                                                                    void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.AcceptActivityInviteCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0),
                        Marshal.PtrToStringUTF8((IntPtr)joinSecret.ptr, (int)joinSecret.size));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(joinSecret.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void SendActivityInviteCallback(ClientResult* result, void* __userData);

            public static void SendActivityInviteCallback_Handler(ClientResult* result,
                                                                  void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.SendActivityInviteCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void ActivityInviteCallback(ActivityInvite* invite, void* __userData);

            public static void ActivityInviteCallback_Handler(ActivityInvite* invite,
                                                              void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.ActivityInviteCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ActivityInvite(*invite, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void ActivityJoinCallback(Discord_String joinSecret, void* __userData);

            public static void ActivityJoinCallback_Handler(Discord_String joinSecret,
                                                            void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.ActivityJoinCallback>(__userData);

                try
                {
                    __callback(Marshal.PtrToStringUTF8((IntPtr)joinSecret.ptr, (int)joinSecret.size));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(joinSecret.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void ActivityJoinWithApplicationCallback(ulong applicationId,
                                                                     Discord_String joinSecret,
                                                                     void* __userData);

            public static void ActivityJoinWithApplicationCallback_Handler(ulong applicationId,
                                                                           Discord_String joinSecret,
                                                                           void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.ActivityJoinWithApplicationCallback>(
                            __userData);

                try
                {
                    __callback(applicationId,
                        Marshal.PtrToStringUTF8((IntPtr)joinSecret.ptr, (int)joinSecret.size));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                    Discord_Free(joinSecret.ptr);
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void UpdateStatusCallback(ClientResult* result, void* __userData);

            public static void UpdateStatusCallback_Handler(ClientResult* result, void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.UpdateStatusCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void UpdateRichPresenceCallback(ClientResult* result, void* __userData);

            public static void UpdateRichPresenceCallback_Handler(ClientResult* result,
                                                                  void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.UpdateRichPresenceCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void UpdateRelationshipCallback(ClientResult* result, void* __userData);

            public static void UpdateRelationshipCallback_Handler(ClientResult* result,
                                                                  void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.UpdateRelationshipCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void SendFriendRequestCallback(ClientResult* result, void* __userData);

            public static void SendFriendRequestCallback_Handler(ClientResult* result,
                                                                 void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.SendFriendRequestCallback>(__userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RelationshipCreatedCallback(ulong userId,
                                                             bool isDiscordRelationshipUpdate,
                                                             void* __userData);

            public static void RelationshipCreatedCallback_Handler(ulong userId,
                                                                   bool isDiscordRelationshipUpdate,
                                                                   void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.RelationshipCreatedCallback>(__userData);

                try
                {
                    __callback(userId, isDiscordRelationshipUpdate);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RelationshipDeletedCallback(ulong userId,
                                                             bool isDiscordRelationshipUpdate,
                                                             void* __userData);

            public static void RelationshipDeletedCallback_Handler(ulong userId,
                                                                   bool isDiscordRelationshipUpdate,
                                                                   void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.RelationshipDeletedCallback>(__userData);

                try
                {
                    __callback(userId, isDiscordRelationshipUpdate);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void GetDiscordClientConnectedUserCallback(ClientResult* result,
                                                                       UserHandle* user,
                                                                       void* __userData);

            public static void GetDiscordClientConnectedUserCallback_Handler(ClientResult* result,
                                                                             UserHandle* user,
                                                                             void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.GetDiscordClientConnectedUserCallback>(
                            __userData);

                try
                {
                    __callback(new Discord.Sdk.ClientResult(*result, 0),
                        user == null ? null : new Discord.Sdk.UserHandle(*user, 0));
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RelationshipGroupsUpdatedCallback(ulong userId, void* __userData);

            public static void RelationshipGroupsUpdatedCallback_Handler(ulong userId,
                                                                         void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.RelationshipGroupsUpdatedCallback>(
                            __userData);

                try
                {
                    __callback(userId);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void UserUpdatedCallback(ulong userId, void* __userData);

            public static void UserUpdatedCallback_Handler(ulong userId, void* __userData)
            {
                var __callback =
                    ManagedUserData
                        .DelegateFromPointer<Discord.Sdk.Client.UserUpdatedCallback>(__userData);

                try
                {
                    __callback(userId);
                }
                catch (Exception ex)
                {
                    __ReportUnhandledException(ex);
                }
                finally
                {
                }
            }

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_Init",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Init(Client* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_InitWithBases",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void InitWithBases(Client* self,
                                                    Discord_String apiBase,
                                                    Discord_String webBase);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_InitWithOptions",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void InitWithOptions(Client* self, ClientCreateOptions* options);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(Client* self);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_AddOrUpdateLobbyMember",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void AddOrUpdateLobbyMember(
          Client* self,
          Discord_String token,
          ulong userId,
          ulong lobbyId,
          Discord.Sdk.NativeMethods.Discord_Properties metadata,
          Discord.Sdk.LobbyMemberFlags flags,
          Discord.Sdk.NativeMethods.Client.UpdateLobbyMemberCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
#endif
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_CreateNewLobby",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void CreateNewLobby(
          Client* self,
          Discord_String token,
          Discord.Sdk.NativeMethods.Client.LobbyActionCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
#endif
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_DeleteLobby",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeleteLobby(
          Client* self,
          Discord_String token,
          ulong lobbyId,
          Discord.Sdk.NativeMethods.Client.LobbyActionCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
#endif
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_EditLobby",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void EditLobby(
          Client* self,
          Discord_String token,
          ulong lobbyId,
          Discord.Sdk.NativeMethods.Discord_Properties lobbyMetadata,
          Discord.Sdk.NativeMethods.Client.LobbyActionCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
#endif
            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_ErrorToString",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void ErrorToString(Discord.Sdk.Client.Error type,
                                                    Discord_String* returnValue);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_ForceCrash_ForTest",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void ForceCrash_ForTest(Client* self);
#endif
            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetApplicationId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong GetApplicationId(Client* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetCurrentUser",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetCurrentUser(Client* self, UserHandle* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetDefaultAudioDeviceId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetDefaultAudioDeviceId(Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetDefaultCommunicationScopes",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetDefaultCommunicationScopes(Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetDefaultPresenceScopes",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetDefaultPresenceScopes(Discord_String* returnValue);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetGuildHandle",
                   CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool GetGuildHandle(Client* self, ulong id, GuildHandle* returnValue);
#endif
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetGuildIds",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetGuildIds(Client* self, Discord_UInt64Span* returnValue);
#endif
            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetVersionHash",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetVersionHash(Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetVersionMajor",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern int GetVersionMajor();

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetVersionMinor",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern int GetVersionMinor();

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetVersionPatch",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern int GetVersionPatch();
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_PerformOnThreadWithString_ForTest",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void PerformOnThreadWithString_ForTest(
          Client* self,
          Discord_String text,
          Discord.Sdk.NativeMethods.Client.PerformOnThreadWithStringCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
#endif
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_RemoveLobbyMember",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void RemoveLobbyMember(
          Client* self,
          Discord_String token,
          ulong userId,
          ulong lobbyId,
          Discord.Sdk.NativeMethods.Client.UpdateLobbyMemberCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
#endif
            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetHttpRequestTimeout",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetHttpRequestTimeout(Client* self,
                                                            int httpTimeoutInMilliseconds);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_StatusToString",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void StatusToString(Discord.Sdk.Client.Status type,
                                                     Discord_String* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_ThreadToString",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void ThreadToString(Discord.Sdk.Client.Thread type,
                                                     Discord_String* returnValue);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_UpdateUserApplicationProfile",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void UpdateUserApplicationProfile(
          Client* self,
          Discord_String token,
          ulong applicationId,
          ulong userId,
          Discord_String providerIssuedUserId,
          Discord_String username,
          Discord.Sdk.NativeMethods.Discord_Properties metadata,
          Discord.Sdk.NativeMethods.Client.UpdateUserApplicationProfileCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
#endif
            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_EndCall",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void EndCall(Client* self,
                                              ulong channelId,
                                              EndCallCallback callback,
                                              void* callback__userDataFree,
                                              void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_EndCalls",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void EndCalls(
                Client* self,
                EndCallsCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetCall",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetCall(Client* self, ulong channelId, Call* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetCalls",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetCalls(Client* self, Discord_CallSpan* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetCurrentInputDevice",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetCurrentInputDevice(
                Client* self,
                GetCurrentInputDeviceCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetCurrentOutputDevice",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetCurrentOutputDevice(
                Client* self,
                GetCurrentOutputDeviceCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetInputDevices",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetInputDevices(
                Client* self,
                GetInputDevicesCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetInputVolume",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern float GetInputVolume(Client* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetOutputDevices",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetOutputDevices(
                Client* self,
                GetOutputDevicesCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetOutputVolume",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern float GetOutputVolume(Client* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetSelfDeafAll",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetSelfDeafAll(Client* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetSelfMuteAll",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetSelfMuteAll(Client* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetAecDump",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetAecDump(Client* self, bool on);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetAutomaticGainControl",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetAutomaticGainControl(Client* self, bool on);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetDeviceChangeCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetDeviceChangeCallback(
                Client* self,
                DeviceChangeCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetEchoCancellation",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetEchoCancellation(Client* self, bool on);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetEngineManagedAudioSession",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetEngineManagedAudioSession(Client* self, bool isEngineManaged);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetInputDevice",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetInputDevice(
                Client* self,
                Discord_String deviceId,
                SetInputDeviceCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetInputVolume",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetInputVolume(Client* self, float inputVolume);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetNoAudioInputCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetNoAudioInputCallback(
                Client* self,
                NoAudioInputCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetNoAudioInputThreshold",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetNoAudioInputThreshold(Client* self, float dBFSThreshold);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetNoiseCancellation",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetNoiseCancellation(Client* self, bool on);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetNoiseSuppression",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetNoiseSuppression(Client* self, bool on);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetOpusHardwareCoding",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetOpusHardwareCoding(Client* self, bool encode, bool decode);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetOutputDevice",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetOutputDevice(
                Client* self,
                Discord_String deviceId,
                SetOutputDeviceCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetOutputVolume",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetOutputVolume(Client* self, float outputVolume);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetSelfDeafAll",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetSelfDeafAll(Client* self, bool deaf);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetSelfMuteAll",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetSelfMuteAll(Client* self, bool mute);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetSpeakerMode",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool SetSpeakerMode(Client* self, bool speakerMode);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetThreadPriority",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetThreadPriority(Client* self,
                                                        Discord.Sdk.Client.Thread thread,
                                                        int priority);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetVoiceParticipantChangedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetVoiceParticipantChangedCallback(
                Client* self,
                VoiceParticipantChangedCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_ShowAudioRoutePicker",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool ShowAudioRoutePicker(Client* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_StartCall",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool StartCall(Client* self, ulong channelId, Call* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_StartCallWithAudioCallbacks",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool StartCallWithAudioCallbacks(
                Client* self,
                ulong lobbyId,
                UserAudioReceivedCallback receivedCb,
                void* receivedCb__userDataFree,
                void* receivedCb__userData,
                UserAudioCapturedCallback capturedCb,
                void* capturedCb__userDataFree,
                void* capturedCb__userData,
                Call* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_AbortAuthorize",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void AbortAuthorize(Client* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_AbortGetTokenFromDevice",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void AbortGetTokenFromDevice(Client* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_Authorize",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Authorize(
                Client* self,
                AuthorizationArgs* args,
                AuthorizationCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_CloseAuthorizeDeviceScreen",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void CloseAuthorizeDeviceScreen(Client* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_CreateAuthorizationCodeVerifier",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void CreateAuthorizationCodeVerifier(
                Client* self,
                AuthorizationCodeVerifier* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_ExchangeChildToken",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void ExchangeChildToken(
                Client* self,
                Discord_String parentApplicationToken,
                ulong childApplicationId,
                ExchangeChildTokenCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_FetchCurrentUser",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void FetchCurrentUser(
                Client* self,
                AuthorizationTokenType tokenType,
                Discord_String token,
                FetchCurrentUserCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetProvisionalToken",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetProvisionalToken(
                Client* self,
                ulong applicationId,
                AuthenticationExternalAuthType externalAuthType,
                Discord_String externalAuthToken,
                TokenExchangeCallback callback,
                void* callback__userDataFree,
                void* callback__userData);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetProvisionalTokenBotAPI",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetProvisionalTokenBotAPI(
          Client* self,
          ulong applicationId,
          Discord_String botToken,
          Discord_String externalUserId,
          Discord_String preferredGlobalName,
          Discord.Sdk.NativeMethods.Client.TokenExchangeCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
#endif
            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetToken",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetToken(
                Client* self,
                ulong applicationId,
                Discord_String code,
                Discord_String codeVerifier,
                Discord_String redirectUri,
                TokenExchangeCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetTokenFromDevice",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetTokenFromDevice(
                Client* self,
                DeviceAuthorizationArgs* args,
                TokenExchangeCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetTokenFromDeviceProvisionalMerge",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetTokenFromDeviceProvisionalMerge(
                Client* self,
                DeviceAuthorizationArgs* args,
                AuthenticationExternalAuthType externalAuthType,
                Discord_String externalAuthToken,
                TokenExchangeCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetTokenFromProvisionalMerge",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetTokenFromProvisionalMerge(
                Client* self,
                ulong applicationId,
                Discord_String code,
                Discord_String codeVerifier,
                Discord_String redirectUri,
                AuthenticationExternalAuthType externalAuthType,
                Discord_String externalAuthToken,
                TokenExchangeCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_IsAuthenticated",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool IsAuthenticated(Client* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_OpenAuthorizeDeviceScreen",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void OpenAuthorizeDeviceScreen(Client* self,
                                                                ulong clientId,
                                                                Discord_String userCode);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_ProvisionalUserMergeCompleted",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void ProvisionalUserMergeCompleted(Client* self, bool success);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_RefreshToken",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void RefreshToken(
                Client* self,
                ulong applicationId,
                Discord_String refreshToken,
                TokenExchangeCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_RegisterAuthorizeRequestCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void RegisterAuthorizeRequestCallback(
                Client* self,
                AuthorizeRequestCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_RemoveAuthorizeRequestCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void RemoveAuthorizeRequestCallback(Client* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_RevokeToken",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void RevokeToken(
                Client* self,
                ulong applicationId,
                Discord_String token,
                RevokeTokenCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetAuthorizeDeviceScreenClosedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetAuthorizeDeviceScreenClosedCallback(
                Client* self,
                AuthorizeDeviceScreenClosedCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetGameWindowPid",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetGameWindowPid(Client* self, int pid);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetTokenExpirationCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetTokenExpirationCallback(
                Client* self,
                TokenExpirationCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_UnmergeIntoProvisionalAccount",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void UnmergeIntoProvisionalAccount(
                Client* self,
                ulong applicationId,
                AuthenticationExternalAuthType externalAuthType,
                Discord_String externalAuthToken,
                UnmergeIntoProvisionalAccountCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_UpdateProvisionalAccountDisplayName",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void UpdateProvisionalAccountDisplayName(
                Client* self,
                Discord_String name,
                UpdateProvisionalAccountDisplayNameCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_UpdateToken",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void UpdateToken(
                Client* self,
                AuthorizationTokenType tokenType,
                Discord_String token,
                UpdateTokenCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_CanOpenMessageInDiscord",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool CanOpenMessageInDiscord(Client* self, ulong messageId);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_DeleteUserMessage",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void DeleteUserMessage(
                Client* self,
                ulong recipientId,
                ulong messageId,
                DeleteUserMessageCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_EditUserMessage",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void EditUserMessage(
                Client* self,
                ulong recipientId,
                ulong messageId,
                Discord_String content,
                EditUserMessageCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetChannelHandle",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetChannelHandle(Client* self,
                                                       ulong channelId,
                                                       ChannelHandle* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetLobbyMessagesWithLimit",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetLobbyMessagesWithLimit(
                Client* self,
                ulong lobbyId,
                int limit,
                GetLobbyMessagesCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetMessageHandle",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetMessageHandle(Client* self,
                                                       ulong messageId,
                                                       MessageHandle* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetUserMessageSummaries",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetUserMessageSummaries(
                Client* self,
                UserMessageSummariesCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetUserMessagesWithLimit",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetUserMessagesWithLimit(
                Client* self,
                ulong recipientId,
                int limit,
                UserMessagesWithLimitCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_OpenMessageInDiscord",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void OpenMessageInDiscord(
                Client* self,
                ulong messageId,
                ProvisionalUserMergeRequiredCallback provisionalUserMergeRequiredCallback,
                void* provisionalUserMergeRequiredCallback__userDataFree,
                void* provisionalUserMergeRequiredCallback__userData,
                OpenMessageInDiscordCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SendLobbyMessage",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SendLobbyMessage(
                Client* self,
                ulong lobbyId,
                Discord_String content,
                SendUserMessageCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SendLobbyMessageWithMetadata",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SendLobbyMessageWithMetadata(
                Client* self,
                ulong lobbyId,
                Discord_String content,
                Discord_Properties metadata,
                SendUserMessageCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SendUserMessage",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SendUserMessage(
                Client* self,
                ulong recipientId,
                Discord_String content,
                SendUserMessageCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SendUserMessageWithMetadata",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SendUserMessageWithMetadata(
                Client* self,
                ulong recipientId,
                Discord_String content,
                Discord_Properties metadata,
                SendUserMessageCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetMessageCreatedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetMessageCreatedCallback(
                Client* self,
                MessageCreatedCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetMessageDeletedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetMessageDeletedCallback(
                Client* self,
                MessageDeletedCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetMessageUpdatedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetMessageUpdatedCallback(
                Client* self,
                MessageUpdatedCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetShowingChat",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetShowingChat(Client* self, bool showingChat);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_AddLogCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void AddLogCallback(
                Client* self,
                LogCallback callback,
                void* callback__userDataFree,
                void* callback__userData,
                LoggingSeverity minSeverity);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_AddVoiceLogCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void AddVoiceLogCallback(
                Client* self,
                LogCallback callback,
                void* callback__userDataFree,
                void* callback__userData,
                LoggingSeverity minSeverity);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_Connect",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Connect(Client* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_Disconnect",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Disconnect(Client* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetStatus",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern Discord.Sdk.Client.Status GetStatus(Client* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_OpenConnectedGamesSettingsInDiscord",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void OpenConnectedGamesSettingsInDiscord(
                Client* self,
                OpenConnectedGamesSettingsInDiscordCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetApplicationId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetApplicationId(Client* self, ulong applicationId);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetLogDir",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool SetLogDir(Client* self,
                                                Discord_String path,
                                                LoggingSeverity minSeverity);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetStatusChangedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetStatusChangedCallback(
                Client* self,
                OnStatusChanged cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetVoiceLogDir",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetVoiceLogDir(Client* self,
                                                     Discord_String path,
                                                     LoggingSeverity minSeverity);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_CreateOrJoinLobby",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void CreateOrJoinLobby(
                Client* self,
                Discord_String secret,
                CreateOrJoinLobbyCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_CreateOrJoinLobbyWithMetadata",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void CreateOrJoinLobbyWithMetadata(
                Client* self,
                Discord_String secret,
                Discord_Properties lobbyMetadata,
                Discord_Properties memberMetadata,
                CreateOrJoinLobbyCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetGuildChannels",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetGuildChannels(
                Client* self,
                ulong guildId,
                GetGuildChannelsCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetLobbyHandle",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetLobbyHandle(Client* self,
                                                     ulong lobbyId,
                                                     LobbyHandle* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetLobbyIds",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetLobbyIds(Client* self, Discord_UInt64Span* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetUserGuilds",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetUserGuilds(
                Client* self,
                GetUserGuildsCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_JoinLinkedLobbyGuild",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void JoinLinkedLobbyGuild(
                Client* self,
                ulong lobbyId,
                ProvisionalUserMergeRequiredCallback provisionalUserMergeRequiredCallback,
                void* provisionalUserMergeRequiredCallback__userDataFree,
                void* provisionalUserMergeRequiredCallback__userData,
                JoinLinkedLobbyGuildCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_LeaveLobby",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void LeaveLobby(
                Client* self,
                ulong lobbyId,
                LeaveLobbyCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_LinkChannelToLobby",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void LinkChannelToLobby(
                Client* self,
                ulong lobbyId,
                ulong channelId,
                LinkOrUnlinkChannelCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetLobbyCreatedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetLobbyCreatedCallback(
                Client* self,
                LobbyCreatedCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetLobbyDeletedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetLobbyDeletedCallback(
                Client* self,
                LobbyDeletedCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetLobbyMemberAddedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetLobbyMemberAddedCallback(
                Client* self,
                LobbyMemberAddedCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetLobbyMemberRemovedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetLobbyMemberRemovedCallback(
                Client* self,
                LobbyMemberRemovedCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetLobbyMemberUpdatedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetLobbyMemberUpdatedCallback(
                Client* self,
                LobbyMemberUpdatedCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetLobbyUpdatedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetLobbyUpdatedCallback(
                Client* self,
                LobbyUpdatedCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_UnlinkChannelFromLobby",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void UnlinkChannelFromLobby(
                Client* self,
                ulong lobbyId,
                LinkOrUnlinkChannelCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_IsDiscordAppInstalled",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void IsDiscordAppInstalled(
                Client* self,
                IsDiscordAppInstalledCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_AcceptActivityInvite",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void AcceptActivityInvite(
                Client* self,
                ActivityInvite* invite,
                AcceptActivityInviteCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_ClearRichPresence",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void ClearRichPresence(Client* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_RegisterLaunchCommand",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool RegisterLaunchCommand(Client* self,
                                                            ulong applicationId,
                                                            Discord_String command);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_RegisterLaunchSteamApplication",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool RegisterLaunchSteamApplication(Client* self,
                                                                     ulong applicationId,
                                                                     uint steamAppId);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SendActivityInvite",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SendActivityInvite(
                Client* self,
                ulong userId,
                Discord_String content,
                SendActivityInviteCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SendActivityJoinRequest",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SendActivityJoinRequest(
                Client* self,
                ulong userId,
                SendActivityInviteCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SendActivityJoinRequestReply",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SendActivityJoinRequestReply(
                Client* self,
                ActivityInvite* invite,
                SendActivityInviteCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetActivityInviteCreatedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetActivityInviteCreatedCallback(
                Client* self,
                ActivityInviteCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetActivityInviteUpdatedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetActivityInviteUpdatedCallback(
                Client* self,
                ActivityInviteCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetActivityJoinCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetActivityJoinCallback(
                Client* self,
                ActivityJoinCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetActivityJoinWithApplicationCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetActivityJoinWithApplicationCallback(
                Client* self,
                ActivityJoinWithApplicationCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetOnlineStatus",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetOnlineStatus(
                Client* self,
                StatusType status,
                UpdateStatusCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_UpdateRichPresence",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void UpdateRichPresence(
                Client* self,
                Activity* activity,
                UpdateRichPresenceCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_AcceptDiscordFriendRequest",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void AcceptDiscordFriendRequest(
                Client* self,
                ulong userId,
                UpdateRelationshipCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_AcceptGameFriendRequest",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void AcceptGameFriendRequest(
                Client* self,
                ulong userId,
                UpdateRelationshipCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_BlockUser",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void BlockUser(
                Client* self,
                ulong userId,
                UpdateRelationshipCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_CancelDiscordFriendRequest",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void CancelDiscordFriendRequest(
                Client* self,
                ulong userId,
                UpdateRelationshipCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_CancelGameFriendRequest",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void CancelGameFriendRequest(
                Client* self,
                ulong userId,
                UpdateRelationshipCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetRelationshipHandle",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetRelationshipHandle(Client* self,
                                                            ulong userId,
                                                            RelationshipHandle* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetRelationships",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetRelationships(Client* self,
                                                       Discord_RelationshipHandleSpan* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetRelationshipsByGroup",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetRelationshipsByGroup(
                Client* self,
                RelationshipGroupType groupType,
                Discord_RelationshipHandleSpan* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_RejectDiscordFriendRequest",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void RejectDiscordFriendRequest(
                Client* self,
                ulong userId,
                UpdateRelationshipCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_RejectGameFriendRequest",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void RejectGameFriendRequest(
                Client* self,
                ulong userId,
                UpdateRelationshipCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_RemoveDiscordAndGameFriend",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void RemoveDiscordAndGameFriend(
                Client* self,
                ulong userId,
                UpdateRelationshipCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_RemoveGameFriend",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void RemoveGameFriend(
                Client* self,
                ulong userId,
                UpdateRelationshipCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SearchFriendsByUsername",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SearchFriendsByUsername(Client* self,
                                                              Discord_String searchStr,
                                                              Discord_UserHandleSpan* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SendDiscordFriendRequest",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SendDiscordFriendRequest(
                Client* self,
                Discord_String username,
                SendFriendRequestCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SendDiscordFriendRequestById",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SendDiscordFriendRequestById(
                Client* self,
                ulong userId,
                UpdateRelationshipCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SendGameFriendRequest",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SendGameFriendRequest(
                Client* self,
                Discord_String username,
                SendFriendRequestCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SendGameFriendRequestById",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SendGameFriendRequestById(
                Client* self,
                ulong userId,
                UpdateRelationshipCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetRelationshipCreatedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetRelationshipCreatedCallback(
                Client* self,
                RelationshipCreatedCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetRelationshipDeletedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetRelationshipDeletedCallback(
                Client* self,
                RelationshipDeletedCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_UnblockUser",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void UnblockUser(
                Client* self,
                ulong userId,
                UpdateRelationshipCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetCurrentUserV2",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetCurrentUserV2(Client* self, UserHandle* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetDiscordClientConnectedUser",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetDiscordClientConnectedUser(
                Client* self,
                ulong applicationId,
                GetDiscordClientConnectedUserCallback callback,
                void* callback__userDataFree,
                void* callback__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_GetUser",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetUser(Client* self, ulong userId, UserHandle* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetRelationshipGroupsUpdatedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetRelationshipGroupsUpdatedCallback(
                Client* self,
                RelationshipGroupsUpdatedCallback cb,
                void* cb__userDataFree,
                void* cb__userData);

            [DllImport(LibraryName,
                EntryPoint = "Discord_Client_SetUserUpdatedCallback",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetUserUpdatedCallback(
                Client* self,
                UserUpdatedCallback cb,
                void* cb__userDataFree,
                void* cb__userData);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CallInfoHandle
        {
            public IntPtr Handle;

            [DllImport(LibraryName,
                EntryPoint = "Discord_CallInfoHandle_Drop",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Drop(CallInfoHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_CallInfoHandle_Clone",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Clone(CallInfoHandle* self, CallInfoHandle* other);

            [DllImport(LibraryName,
                EntryPoint = "Discord_CallInfoHandle_ChannelId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong ChannelId(CallInfoHandle* self);

            [DllImport(LibraryName,
                EntryPoint = "Discord_CallInfoHandle_GetParticipants",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void GetParticipants(CallInfoHandle* self,
                                                      Discord_UInt64Span* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_CallInfoHandle_GetVoiceStateHandle",
                CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetVoiceStateHandle(CallInfoHandle* self,
                                                          ulong userId,
                                                          VoiceStateHandle* returnValue);

            [DllImport(LibraryName,
                EntryPoint = "Discord_CallInfoHandle_GuildId",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong GuildId(CallInfoHandle* self);
        }
    }

    /// <summary>
    ///  When one user invites another to join their game on Discord, it will send a message to that
    ///  user. The SDK will parse those messages for you automatically, and this struct contains all of
    ///  the relevant invite information which is needed to later accept that invite.
    /// </summary>
    public class ActivityInvite : IDisposable
    {
        internal NativeMethods.ActivityInvite self;
        private int disposed_;

        internal ActivityInvite(NativeMethods.ActivityInvite self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~ActivityInvite() { Dispose(); }

        public ActivityInvite()
        {
            NativeMethods.__Init();

            unsafe
            {
                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    NativeMethods.ActivityInvite.Init(self);
                }
            }

            NativeMethods.__OnPostConstruct(this);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    NativeMethods.ActivityInvite.Drop(self);
                }
            }
        }

        public ActivityInvite(ActivityInvite other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityInvite* otherPtr = &other.self)
                {
                    fixed (NativeMethods.ActivityInvite* selfPtr = &self)
                    {
                        NativeMethods.ActivityInvite.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe ActivityInvite(NativeMethods.ActivityInvite* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.ActivityInvite* selfPtr = &self)
                {
                    NativeMethods.ActivityInvite.Clone(selfPtr, otherPtr);
                }
            }
        }

        public ulong SenderId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    __returnValue = NativeMethods.ActivityInvite.SenderId(self);
                }

                return __returnValue;
            }
        }

        public void SetSenderId(ulong value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    NativeMethods.ActivityInvite.SetSenderId(self, value);
                }
            }
        }

        public ulong ChannelId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    __returnValue = NativeMethods.ActivityInvite.ChannelId(self);
                }

                return __returnValue;
            }
        }

        public void SetChannelId(ulong value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    NativeMethods.ActivityInvite.SetChannelId(self, value);
                }
            }
        }

        public ulong MessageId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    __returnValue = NativeMethods.ActivityInvite.MessageId(self);
                }

                return __returnValue;
            }
        }

        public void SetMessageId(ulong value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    NativeMethods.ActivityInvite.SetMessageId(self, value);
                }
            }
        }

        public ActivityActionTypes Type()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                ActivityActionTypes __returnValue;

                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    __returnValue = NativeMethods.ActivityInvite.Type(self);
                }

                return __returnValue;
            }
        }

        public void SetType(ActivityActionTypes value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    NativeMethods.ActivityInvite.SetType(self, value);
                }
            }
        }

        public ulong ApplicationId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    __returnValue = NativeMethods.ActivityInvite.ApplicationId(self);
                }

                return __returnValue;
            }
        }

        public void SetApplicationId(ulong value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    NativeMethods.ActivityInvite.SetApplicationId(self, value);
                }
            }
        }

        public ulong ParentApplicationId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    __returnValue = NativeMethods.ActivityInvite.ParentApplicationId(self);
                }

                return __returnValue;
            }
        }

        public void SetParentApplicationId(ulong value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    NativeMethods.ActivityInvite.SetParentApplicationId(self, value);
                }
            }
        }

        public string PartyId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    NativeMethods.ActivityInvite.PartyId(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetPartyId(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    NativeMethods.ActivityInvite.SetPartyId(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public string SessionId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    NativeMethods.ActivityInvite.SessionId(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetSessionId(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    NativeMethods.ActivityInvite.SetSessionId(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public bool IsValid()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    __returnValue = NativeMethods.ActivityInvite.IsValid(self);
                }

                return __returnValue;
            }
        }

        public void SetIsValid(bool value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityInvite));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityInvite* self = &this.self)
                {
                    NativeMethods.ActivityInvite.SetIsValid(self, value);
                }
            }
        }
    }

    /// <summary>
    ///  Struct which controls what your rich presence looks like in
    ///  the Discord client. If you don't specify any values, the icon
    ///  and name of your application will be used as defaults.
    /// </summary>
    /// <remarks>
    ///  Image assets can be either the unique identifier for an image
    ///  you uploaded to your application via the `Rich Presence` page in
    ///  the Developer portal, or they can be an external image URL.
    ///
    ///  As an example, if I uploaded an asset and name it `goofy-icon`,
    ///  I could set either image field to the string `goofy-icon`. Alternatively,
    ///  if my icon was hosted at `http://my-site.com/goofy.jpg`, I could
    ///  pass that URL into either image field.
    ///
    ///  See https://discord.com/developers/docs/rich-presence/overview#adding-custom-art-assets
    ///  for more information on using custom art assets, as well as for visual
    ///  examples of what each field does.
    ///
    /// </remarks>
    public class ActivityAssets : IDisposable
    {
        internal NativeMethods.ActivityAssets self;
        private int disposed_;

        internal ActivityAssets(NativeMethods.ActivityAssets self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~ActivityAssets() { Dispose(); }

        public ActivityAssets()
        {
            NativeMethods.__Init();

            unsafe
            {
                fixed (NativeMethods.ActivityAssets* self = &this.self)
                {
                    NativeMethods.ActivityAssets.Init(self);
                }
            }

            NativeMethods.__OnPostConstruct(this);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.ActivityAssets* self = &this.self)
                {
                    NativeMethods.ActivityAssets.Drop(self);
                }
            }
        }

        public ActivityAssets(ActivityAssets other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityAssets));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityAssets* otherPtr = &other.self)
                {
                    fixed (NativeMethods.ActivityAssets* selfPtr = &self)
                    {
                        NativeMethods.ActivityAssets.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe ActivityAssets(NativeMethods.ActivityAssets* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.ActivityAssets* selfPtr = &self)
                {
                    NativeMethods.ActivityAssets.Clone(selfPtr, otherPtr);
                }
            }
        }

        public string? LargeImage()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityAssets));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ActivityAssets* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.ActivityAssets.LargeImage(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetLargeImage(string? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityAssets));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned = NativeMethods.__InitNullableStringLocal(
                    __scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ActivityAssets* self = &this.self)
                {
                    NativeMethods.ActivityAssets.SetLargeImage(self,
                        (value != null ? &__valueSpan : null));
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public string? LargeText()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityAssets));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ActivityAssets* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.ActivityAssets.LargeText(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetLargeText(string? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityAssets));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned = NativeMethods.__InitNullableStringLocal(
                    __scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ActivityAssets* self = &this.self)
                {
                    NativeMethods.ActivityAssets.SetLargeText(self,
                        (value != null ? &__valueSpan : null));
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public string? LargeUrl()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityAssets));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ActivityAssets* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.ActivityAssets.LargeUrl(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetLargeUrl(string? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityAssets));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned = NativeMethods.__InitNullableStringLocal(
                    __scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ActivityAssets* self = &this.self)
                {
                    NativeMethods.ActivityAssets.SetLargeUrl(self,
                        (value != null ? &__valueSpan : null));
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public string? SmallImage()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityAssets));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ActivityAssets* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.ActivityAssets.SmallImage(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetSmallImage(string? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityAssets));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned = NativeMethods.__InitNullableStringLocal(
                    __scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ActivityAssets* self = &this.self)
                {
                    NativeMethods.ActivityAssets.SetSmallImage(self,
                        (value != null ? &__valueSpan : null));
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public string? SmallText()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityAssets));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ActivityAssets* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.ActivityAssets.SmallText(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetSmallText(string? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityAssets));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned = NativeMethods.__InitNullableStringLocal(
                    __scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ActivityAssets* self = &this.self)
                {
                    NativeMethods.ActivityAssets.SetSmallText(self,
                        (value != null ? &__valueSpan : null));
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public string? SmallUrl()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityAssets));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ActivityAssets* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.ActivityAssets.SmallUrl(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetSmallUrl(string? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityAssets));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned = NativeMethods.__InitNullableStringLocal(
                    __scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ActivityAssets* self = &this.self)
                {
                    NativeMethods.ActivityAssets.SetSmallUrl(self,
                        (value != null ? &__valueSpan : null));
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public string? InviteCoverImage()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityAssets));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ActivityAssets* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.ActivityAssets.InviteCoverImage(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetInviteCoverImage(string? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityAssets));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned = NativeMethods.__InitNullableStringLocal(
                    __scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ActivityAssets* self = &this.self)
                {
                    NativeMethods.ActivityAssets.SetInviteCoverImage(
                        self, (value != null ? &__valueSpan : null));
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }
    }

    /// <summary>
    /// \see Activity
    /// </summary>
    public class ActivityTimestamps : IDisposable
    {
        internal NativeMethods.ActivityTimestamps self;
        private int disposed_;

        internal ActivityTimestamps(NativeMethods.ActivityTimestamps self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~ActivityTimestamps() { Dispose(); }

        public ActivityTimestamps()
        {
            NativeMethods.__Init();

            unsafe
            {
                fixed (NativeMethods.ActivityTimestamps* self = &this.self)
                {
                    NativeMethods.ActivityTimestamps.Init(self);
                }
            }

            NativeMethods.__OnPostConstruct(this);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.ActivityTimestamps* self = &this.self)
                {
                    NativeMethods.ActivityTimestamps.Drop(self);
                }
            }
        }

        public ActivityTimestamps(ActivityTimestamps other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityTimestamps));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityTimestamps* otherPtr = &other.self)
                {
                    fixed (NativeMethods.ActivityTimestamps* selfPtr = &self)
                    {
                        NativeMethods.ActivityTimestamps.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe ActivityTimestamps(NativeMethods.ActivityTimestamps* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.ActivityTimestamps* selfPtr = &self)
                {
                    NativeMethods.ActivityTimestamps.Clone(selfPtr, otherPtr);
                }
            }
        }

        public ulong Start()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityTimestamps));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.ActivityTimestamps* self = &this.self)
                {
                    __returnValue = NativeMethods.ActivityTimestamps.Start(self);
                }

                return __returnValue;
            }
        }

        public void SetStart(ulong value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityTimestamps));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityTimestamps* self = &this.self)
                {
                    NativeMethods.ActivityTimestamps.SetStart(self, value);
                }
            }
        }

        public ulong End()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityTimestamps));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.ActivityTimestamps* self = &this.self)
                {
                    __returnValue = NativeMethods.ActivityTimestamps.End(self);
                }

                return __returnValue;
            }
        }

        public void SetEnd(ulong value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityTimestamps));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityTimestamps* self = &this.self)
                {
                    NativeMethods.ActivityTimestamps.SetEnd(self, value);
                }
            }
        }
    }

    /// <summary>
    /// \see Activity
    /// </summary>
    public class ActivityParty : IDisposable
    {
        internal NativeMethods.ActivityParty self;
        private int disposed_;

        internal ActivityParty(NativeMethods.ActivityParty self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~ActivityParty() { Dispose(); }

        public ActivityParty()
        {
            NativeMethods.__Init();

            unsafe
            {
                fixed (NativeMethods.ActivityParty* self = &this.self)
                {
                    NativeMethods.ActivityParty.Init(self);
                }
            }

            NativeMethods.__OnPostConstruct(this);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.ActivityParty* self = &this.self)
                {
                    NativeMethods.ActivityParty.Drop(self);
                }
            }
        }

        public ActivityParty(ActivityParty other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityParty));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityParty* otherPtr = &other.self)
                {
                    fixed (NativeMethods.ActivityParty* selfPtr = &self)
                    {
                        NativeMethods.ActivityParty.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe ActivityParty(NativeMethods.ActivityParty* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.ActivityParty* selfPtr = &self)
                {
                    NativeMethods.ActivityParty.Clone(selfPtr, otherPtr);
                }
            }
        }

        public string Id()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityParty));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ActivityParty* self = &this.self)
                {
                    NativeMethods.ActivityParty.Id(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetId(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityParty));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ActivityParty* self = &this.self)
                {
                    NativeMethods.ActivityParty.SetId(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public int CurrentSize()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityParty));
            }

            unsafe
            {
                int __returnValue;

                fixed (NativeMethods.ActivityParty* self = &this.self)
                {
                    __returnValue = NativeMethods.ActivityParty.CurrentSize(self);
                }

                return __returnValue;
            }
        }

        public void SetCurrentSize(int value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityParty));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityParty* self = &this.self)
                {
                    NativeMethods.ActivityParty.SetCurrentSize(self, value);
                }
            }
        }

        public int MaxSize()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityParty));
            }

            unsafe
            {
                int __returnValue;

                fixed (NativeMethods.ActivityParty* self = &this.self)
                {
                    __returnValue = NativeMethods.ActivityParty.MaxSize(self);
                }

                return __returnValue;
            }
        }

        public void SetMaxSize(int value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityParty));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityParty* self = &this.self)
                {
                    NativeMethods.ActivityParty.SetMaxSize(self, value);
                }
            }
        }

        public ActivityPartyPrivacy Privacy()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityParty));
            }

            unsafe
            {
                ActivityPartyPrivacy __returnValue;

                fixed (NativeMethods.ActivityParty* self = &this.self)
                {
                    __returnValue = NativeMethods.ActivityParty.Privacy(self);
                }

                return __returnValue;
            }
        }

        public void SetPrivacy(ActivityPartyPrivacy value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityParty));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityParty* self = &this.self)
                {
                    NativeMethods.ActivityParty.SetPrivacy(self, value);
                }
            }
        }
    }

    /// <summary>
    /// \see Activity
    /// </summary>
    public class ActivitySecrets : IDisposable
    {
        internal NativeMethods.ActivitySecrets self;
        private int disposed_;

        internal ActivitySecrets(NativeMethods.ActivitySecrets self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~ActivitySecrets() { Dispose(); }

        public ActivitySecrets()
        {
            NativeMethods.__Init();

            unsafe
            {
                fixed (NativeMethods.ActivitySecrets* self = &this.self)
                {
                    NativeMethods.ActivitySecrets.Init(self);
                }
            }

            NativeMethods.__OnPostConstruct(this);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.ActivitySecrets* self = &this.self)
                {
                    NativeMethods.ActivitySecrets.Drop(self);
                }
            }
        }

        public ActivitySecrets(ActivitySecrets other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivitySecrets));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.ActivitySecrets* otherPtr = &other.self)
                {
                    fixed (NativeMethods.ActivitySecrets* selfPtr = &self)
                    {
                        NativeMethods.ActivitySecrets.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe ActivitySecrets(NativeMethods.ActivitySecrets* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.ActivitySecrets* selfPtr = &self)
                {
                    NativeMethods.ActivitySecrets.Clone(selfPtr, otherPtr);
                }
            }
        }

        public string Join()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivitySecrets));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ActivitySecrets* self = &this.self)
                {
                    NativeMethods.ActivitySecrets.Join(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetJoin(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivitySecrets));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ActivitySecrets* self = &this.self)
                {
                    NativeMethods.ActivitySecrets.SetJoin(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }
    }

    /// <summary>
    /// \see Activity
    /// </summary>
    public class ActivityButton : IDisposable
    {
        internal NativeMethods.ActivityButton self;
        private int disposed_;

        internal ActivityButton(NativeMethods.ActivityButton self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~ActivityButton() { Dispose(); }

        public ActivityButton()
        {
            NativeMethods.__Init();

            unsafe
            {
                fixed (NativeMethods.ActivityButton* self = &this.self)
                {
                    NativeMethods.ActivityButton.Init(self);
                }
            }

            NativeMethods.__OnPostConstruct(this);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.ActivityButton* self = &this.self)
                {
                    NativeMethods.ActivityButton.Drop(self);
                }
            }
        }

        public ActivityButton(ActivityButton other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityButton));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityButton* otherPtr = &other.self)
                {
                    fixed (NativeMethods.ActivityButton* selfPtr = &self)
                    {
                        NativeMethods.ActivityButton.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe ActivityButton(NativeMethods.ActivityButton* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.ActivityButton* selfPtr = &self)
                {
                    NativeMethods.ActivityButton.Clone(selfPtr, otherPtr);
                }
            }
        }

        public string Label()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityButton));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ActivityButton* self = &this.self)
                {
                    NativeMethods.ActivityButton.Label(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetLabel(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityButton));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ActivityButton* self = &this.self)
                {
                    NativeMethods.ActivityButton.SetLabel(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public string Url()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityButton));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ActivityButton* self = &this.self)
                {
                    NativeMethods.ActivityButton.Url(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetUrl(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ActivityButton));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ActivityButton* self = &this.self)
                {
                    NativeMethods.ActivityButton.SetUrl(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }
    }

    /// <summary>
    ///  An Activity represents one "thing" a user is doing on Discord and is part of their rich
    ///  presence.
    /// </summary>
    /// <remarks>
    ///  Additional information is located on the Discord Developer Portal:
    ///  - https://discord.com/developers/docs/rich-presence/overview
    ///  - https://discord.com/developers/docs/developer-tools/game-sdk#activities
    ///  - https://discord.com/developers/docs/rich-presence/best-practices
    ///
    ///  While RichPresence supports multiple types of activities, the only activity type that is really
    ///  relevant for the SDK is ActivityTypes::Playing. Additionally, the SDK will only expose
    ///  Activities that are associated with the current game (or application). So for example, a field
    ///  like `name` below, will always be set to the current game's name from the view of the SDK.
    ///
    ///  ## Customization
    ///  When an activity shows up on Discord, it will look something like this:
    ///  1. Playing "game name"
    ///  2. Capture the flag | 2 - 1
    ///  3. In a group (2 of 3)
    ///
    ///  You can control how lines 2 and 3 are rendered in Discord, here's the breakdown:
    ///  - Line 1, `Playing "game name"` is powered by the name of your game (or application) on
    ///  Discord.
    ///  - Line 2, `Capture the flag | 2 - 1` is powered by the `details` field in the activity, and
    ///  this should generally try to describe what the _player_ is currently doing. You can even
    ///  include dynamic data such as a match score here.
    ///  - Line 3, `In a group (2 of 3)` describes the _party_ the player is in. "Party" is used to
    ///  refer to a group of players in a shared context, such as a lobby, server, team, etc. The first
    ///  half, `In a group` is powered by the `state` field in the activity, and the second half, `(2 of
    ///  3)` is powered by the `party` field in the activity and describes how many people are in the
    ///  current party and how big the party can get.
    ///
    ///  This diagram visually shows the field mapping:
    ///
    ///
    ///  \image html "rich_presence.png" "Rich presence field diagram" width=1070px
    ///
    ///  You can also specify up to two custom buttons to display on the rich presence.
    ///  These buttons will open the URL in the user's default browser.
    ///
    ///  \code
    ///      discordpp::ActivityButton button;
    ///      button.SetLabel("Button 1");
    ///      button.SetUrl("https://example.com");
    ///      activity.AddButton(button);
    ///  \endcode
    ///
    ///
    ///  ## Invites / Joinable Activities
    ///  Other users can be invited to join the current player's activity (or request to join it too),
    ///  but that does require certain fields to be set:
    ///  1. ActivityParty must be set and have a non-empty ActivityParty::Id field. All users in the
    ///  party should set the same id field too!
    ///  2. ActivityParty must specify the size of the group, and there must be room in the group for
    ///  another person to join.
    ///  3. ActivitySecrets::Join must be set to a non-empty string. The join secret is only shared with
    ///  users who are accepted into the party by an existing member, so it is truly a secret. You can
    ///  use this so that when the user is accepted your game knows how to join them to the party. For
    ///  example it could be an internal game ID, or a Discord lobby ID/secret that the client could
    ///  join.
    ///
    ///  There is additional information about game invites here:
    ///  https://support.discord.com/hc/en-us/articles/115001557452-Game-Invites
    ///
    ///  ### Mobile Invites
    ///  Activity invites are handled via a deep link. To enable users to join your game via an invite
    ///  in the Discord client, you must do two things:
    ///  1. Set your deep link URL in the Discord developer portal. This will be available on the
    ///  General tab of your application once Social Layer integration is enabled for your app.
    ///  2. Set the desired supported platforms when reporting the activity info in your rich presence,
    ///  e.g.:
    ///
    ///
    ///  \code
    ///      activity.SetSupportedPlatforms(
    ///          ActivityGamePlatforms.Desktop |
    ///          ActivityGamePlatforms.IOS |
    ///          ActivityGamePlatforms.Android);
    ///  \endcode
    ///
    ///
    ///  When the user accepts the invite, the Discord client will open:
    ///  `[your url]/_discord/join?secret=[the join secret you set]`
    ///
    ///  ### Example Invites Flow
    ///  If you are using Discord lobbies for your game, a neat flow would look like this:
    ///  - When a user starts playing the game, they create a lobby with a random secret string, using
    ///  Client::CreateOrJoinLobby
    ///  - That user publishes their RichPresence with the join secret set to the lobby secret, along
    ///  with party size information
    ///  - Another use can then see that RichPresence on Discord and join off of it
    ///  - Once accepted the new user receives the join secret and their client can call
    ///  CreateOrJoinLobby(joinSecret) to join the lobby
    ///  - Finally the original user can notice that the lobby membership has changed and so they
    ///  publish a new RichPresence update containing the updating party size information.
    ///
    ///  ### Invites Code Example
    ///
    ///  \code
    ///  // User A
    ///  // 1. Create a lobby with secret
    ///  std::string lobbySecret = "foo";
    ///  client->CreateOrJoinLobby(lobbySecret, [=](discordpp::ClientResult result, uint64_t lobbyId) {
    ///      // 2. Update rich presence with join secret
    ///      discordpp::Activity activity{};
    ///      // set name, state, party size ...
    ///      discordpp::ActivitySecrets secrets{};
    ///      secrets.SetJoin(lobbySecret);
    ///      activity.SetSecrets(secrets);
    ///      client->UpdateRichPresence(std::move(activity), [](discordpp::ClientResult result) {});
    ///  });
    ///  // 3. Some time later, send an invite
    ///  client->SendActivityInvite(USER_B_ID, "come play with me", [](auto result) {});
    ///
    ///  // User B
    ///  // 4. Monitor for new invites. Alternatively, you can use
    ///  // Client::SetActivityInviteUpdatedCallback to get updates on existing invites.
    ///  client->SetActivityInviteCreatedCallback([client](auto invite) {
    ///      // 5. When an invite is received, ask the user if they want to accept it.
    ///      // If they choose to do so then go ahead and invoke AcceptActivityInvite
    ///      client->AcceptActivityInvite(invite,
    ///          [client](discordpp::ClientResult result, std::string secret) {
    ///          if (result.Successful()) {
    ///              // 5. Join the lobby using the joinSecret
    ///              client->CreateOrJoinLobby(secret, [](discordpp::ClientResult result, uint64_t
    ///              lobbyId) {
    ///                  // Successfully joined lobby!
    ///              });
    ///          }
    ///      });
    ///  });
    ///  \endcode
    ///
    ///
    ///  ### Join Requests Code Example
    ///  Users can also request to join each others parties. This code snippet shows how that flow might
    ///  look:
    ///
    ///  \code
    ///  // User A
    ///  // 1. Create a lobby with secret
    ///  std::string lobbySecret = "foo";
    ///  client->CreateOrJoinLobby(lobbySecret, [=](discordpp::ClientResult result, uint64_t lobbyId) {
    ///      // 2. Update rich presence with join secret
    ///      discordpp::Activity activity{};
    ///      // set name, state, party size ...
    ///      discordpp::ActivitySecrets secrets{};
    ///      secrets.SetJoin(lobbySecret);
    ///      activity.SetSecrets(secrets);
    ///      client->UpdateRichPresence(std::move(activity), [](discordpp::ClientResult result) {});
    ///  });
    ///
    ///  // User B
    ///  // 3. Request to join User A's party
    ///  client->SendActivityJoinRequest(USER_A_ID, [](auto result) {});
    ///
    ///  // User A
    ///  // Monitor for new invites:
    ///  client->SetActivityInviteCreatedCallback([client](auto invite) {
    ///      // 5. The game can now show that User A has received a request to join their party
    ///      // If User A is ok with that, they can reply back:
    ///      // Note: invite.type will be ActivityActionTypes::JoinRequest in this example
    ///      client->SendActivityJoinRequestReply(invite, [](auto result) {});
    ///  });
    ///
    ///  // User B
    ///  // 6. Same as before, user B can monitor for invites
    ///  client->SetActivityInviteCreatedCallback([client](auto invite) {
    ///      // 7. When an invite is received, ask the user if they want to accept it.
    ///      // If they choose to do so then go ahead and invoke AcceptActivityInvite
    ///      client->AcceptActivityInvite(invite,
    ///          [client](discordpp::ClientResult result, std::string secret) {
    ///          if (result.Successful()) {
    ///              // 5. Join the lobby using the joinSecret
    ///              client->CreateOrJoinLobby(secret, [](auto result, uint64_t lobbyId) {
    ///                  // Successfully joined lobby!
    ///              });
    ///          }
    ///      });
    ///  });
    ///  \endcode
    ///
    ///
    /// </remarks>
    public class Activity : IDisposable
    {
        internal NativeMethods.Activity self;
        private int disposed_;

        internal Activity(NativeMethods.Activity self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~Activity() { Dispose(); }

        public Activity()
        {
            NativeMethods.__Init();

            unsafe
            {
                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.Init(self);
                }
            }

            NativeMethods.__OnPostConstruct(this);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.Drop(self);
                }
            }
        }

        public Activity(Activity other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.Activity* otherPtr = &other.self)
                {
                    fixed (NativeMethods.Activity* selfPtr = &self)
                    {
                        NativeMethods.Activity.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe Activity(NativeMethods.Activity* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.Activity* selfPtr = &self)
                {
                    NativeMethods.Activity.Clone(selfPtr, otherPtr);
                }
            }
        }

        /// <summary>
        ///  Adds a custom button to the rich presence
        /// </summary>
        public void AddButton(ActivityButton button)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityButton* __buttonFixed = &button.self)
                {
                    fixed (NativeMethods.Activity* self = &this.self)
                    {
                        NativeMethods.Activity.AddButton(self, __buttonFixed);
                    }
                }
            }
        }

        /// <summary>
        ///  Compares each field of the Activity struct for equality.
        /// </summary>
        public bool Equals(Activity other)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.Activity* __otherFixed = &other.self)
                {
                    fixed (NativeMethods.Activity* self = &this.self)
                    {
                        __returnValue = NativeMethods.Activity.Equals(self, __otherFixed);
                    }
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the custom buttons for the rich presence
        /// </summary>
        public ActivityButton[] GetButtons()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_ActivityButtonSpan();

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.GetButtons(self, &__returnValue);
                }

                var __returnValueSurface = new ActivityButton[(int)__returnValue.size];

                for (int __i = 0; __i < (int)__returnValue.size; __i++)
                {
                    __returnValueSurface[__i] = new ActivityButton(__returnValue.ptr[__i], 0);
                }

                NativeMethods.Discord_Free(__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public string Name()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.Name(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetName(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.SetName(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public ActivityTypes Type()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                ActivityTypes __returnValue;

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    __returnValue = NativeMethods.Activity.Type(self);
                }

                return __returnValue;
            }
        }

        public void SetType(ActivityTypes value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.SetType(self, value);
                }
            }
        }

        public StatusDisplayTypes? StatusDisplayType()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                bool __returnIsNonNull;
                StatusDisplayTypes __returnValue;

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.Activity.StatusDisplayType(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                return __returnValue;
            }
        }

        public void SetStatusDisplayType(StatusDisplayTypes? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                var __valueLocal = value ?? default;

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.SetStatusDisplayType(self,
                        (value != null ? &__valueLocal : null));
                }
            }
        }

        public string? State()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.Activity.State(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetState(string? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned = NativeMethods.__InitNullableStringLocal(
                    __scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.SetState(self, (value != null ? &__valueSpan : null));
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public string? StateUrl()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.Activity.StateUrl(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetStateUrl(string? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned = NativeMethods.__InitNullableStringLocal(
                    __scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.SetStateUrl(self, (value != null ? &__valueSpan : null));
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public string? Details()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.Activity.Details(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetDetails(string? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned = NativeMethods.__InitNullableStringLocal(
                    __scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.SetDetails(self, (value != null ? &__valueSpan : null));
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public string? DetailsUrl()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.Activity.DetailsUrl(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetDetailsUrl(string? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned = NativeMethods.__InitNullableStringLocal(
                    __scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.SetDetailsUrl(self, (value != null ? &__valueSpan : null));
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public ulong? ApplicationId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                bool __returnIsNonNull;
                ulong __returnValue;

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.Activity.ApplicationId(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                return __returnValue;
            }
        }

        public void SetApplicationId(ulong? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                var __valueLocal = value ?? default;

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.SetApplicationId(self,
                        (value != null ? &__valueLocal : null));
                }
            }
        }

        public ulong? ParentApplicationId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                bool __returnIsNonNull;
                ulong __returnValue;

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.Activity.ParentApplicationId(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                return __returnValue;
            }
        }

        public void SetParentApplicationId(ulong? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                var __valueLocal = value ?? default;

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.SetParentApplicationId(
                        self, (value != null ? &__valueLocal : null));
                }
            }
        }

        public ActivityAssets? Assets()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.ActivityAssets();
                ActivityAssets? __returnValue = null;

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.Activity.Assets(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new ActivityAssets(__returnValueNative, 0);
                return __returnValue;
            }
        }

        public void SetAssets(ActivityAssets? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                var __valueLocal = value?.self ?? default;

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.SetAssets(self, (value != null ? &__valueLocal : null));
                }

                if (value != null)
                {
                    value.self = __valueLocal;
                }
            }
        }

        public ActivityTimestamps? Timestamps()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.ActivityTimestamps();
                ActivityTimestamps? __returnValue = null;

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.Activity.Timestamps(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new ActivityTimestamps(__returnValueNative, 0);
                return __returnValue;
            }
        }

        public void SetTimestamps(ActivityTimestamps? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                var __valueLocal = value?.self ?? default;

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.SetTimestamps(self, (value != null ? &__valueLocal : null));
                }

                if (value != null)
                {
                    value.self = __valueLocal;
                }
            }
        }

        public ActivityParty? Party()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.ActivityParty();
                ActivityParty? __returnValue = null;

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.Activity.Party(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new ActivityParty(__returnValueNative, 0);
                return __returnValue;
            }
        }

        public void SetParty(ActivityParty? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                var __valueLocal = value?.self ?? default;

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.SetParty(self, (value != null ? &__valueLocal : null));
                }

                if (value != null)
                {
                    value.self = __valueLocal;
                }
            }
        }

        public ActivitySecrets? Secrets()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.ActivitySecrets();
                ActivitySecrets? __returnValue = null;

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.Activity.Secrets(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new ActivitySecrets(__returnValueNative, 0);
                return __returnValue;
            }
        }

        public void SetSecrets(ActivitySecrets? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                var __valueLocal = value?.self ?? default;

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.SetSecrets(self, (value != null ? &__valueLocal : null));
                }

                if (value != null)
                {
                    value.self = __valueLocal;
                }
            }
        }

        public ActivityGamePlatforms SupportedPlatforms()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                ActivityGamePlatforms __returnValue;

                fixed (NativeMethods.Activity* self = &this.self)
                {
                    __returnValue = NativeMethods.Activity.SupportedPlatforms(self);
                }

                return __returnValue;
            }
        }

        public void SetSupportedPlatforms(ActivityGamePlatforms value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Activity));
            }

            unsafe
            {
                fixed (NativeMethods.Activity* self = &this.self)
                {
                    NativeMethods.Activity.SetSupportedPlatforms(self, value);
                }
            }
        }
    }

    /// <summary>
    ///  Struct that stores information about the result of an SDK function call.
    /// </summary>
    /// <remarks>
    ///  Functions can fail for a few reasons including:
    ///  - The Client is not yet ready and able to perform the action.
    ///  - The inputs passed to the function are invalid.
    ///  - The function makes an API call to Discord's backend which returns an error.
    ///  - The user is offline.
    ///
    ///  The ClientResult::Type field is used to to distinguish between the above types of failures
    ///
    /// </remarks>
    public class ClientResult : IDisposable
    {
        internal NativeMethods.ClientResult self;
        private int disposed_;

        internal ClientResult(NativeMethods.ClientResult self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~ClientResult() { Dispose(); }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    NativeMethods.ClientResult.Drop(self);
                }
            }
        }

        public ClientResult(ClientResult other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.ClientResult* otherPtr = &other.self)
                {
                    fixed (NativeMethods.ClientResult* selfPtr = &self)
                    {
                        NativeMethods.ClientResult.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe ClientResult(NativeMethods.ClientResult* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.ClientResult* selfPtr = &self)
                {
                    NativeMethods.ClientResult.Clone(selfPtr, otherPtr);
                }
            }
        }

        /// <summary>
        ///  Returns the error message if any of the ClientResult.
        /// </summary>
        public override string ToString()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    NativeMethods.ClientResult.ToString(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public ErrorType Type()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                ErrorType __returnValue;

                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    __returnValue = NativeMethods.ClientResult.Type(self);
                }

                return __returnValue;
            }
        }

        public void SetType(ErrorType value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    NativeMethods.ClientResult.SetType(self, value);
                }
            }
        }

        public string Error()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    NativeMethods.ClientResult.Error(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetError(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    NativeMethods.ClientResult.SetError(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public int ErrorCode()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                int __returnValue;

                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    __returnValue = NativeMethods.ClientResult.ErrorCode(self);
                }

                return __returnValue;
            }
        }

        public void SetErrorCode(int value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    NativeMethods.ClientResult.SetErrorCode(self, value);
                }
            }
        }

        public HttpStatusCode Status()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                HttpStatusCode __returnValue;

                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    __returnValue = NativeMethods.ClientResult.Status(self);
                }

                return __returnValue;
            }
        }

        public void SetStatus(HttpStatusCode value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    NativeMethods.ClientResult.SetStatus(self, value);
                }
            }
        }

        public string ResponseBody()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    NativeMethods.ClientResult.ResponseBody(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetResponseBody(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    NativeMethods.ClientResult.SetResponseBody(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public bool Successful()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    __returnValue = NativeMethods.ClientResult.Successful(self);
                }

                return __returnValue;
            }
        }

        public void SetSuccessful(bool value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    NativeMethods.ClientResult.SetSuccessful(self, value);
                }
            }
        }

        public bool Retryable()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    __returnValue = NativeMethods.ClientResult.Retryable(self);
                }

                return __returnValue;
            }
        }

        public void SetRetryable(bool value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    NativeMethods.ClientResult.SetRetryable(self, value);
                }
            }
        }

        public float RetryAfter()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                float __returnValue;

                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    __returnValue = NativeMethods.ClientResult.RetryAfter(self);
                }

                return __returnValue;
            }
        }

        public void SetRetryAfter(float value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientResult));
            }

            unsafe
            {
                fixed (NativeMethods.ClientResult* self = &this.self)
                {
                    NativeMethods.ClientResult.SetRetryAfter(self, value);
                }
            }
        }
    }

    /// <summary>
    ///  Struct that encapsulates the challenge part of the code verification flow.
    /// </summary>
    public class AuthorizationCodeChallenge : IDisposable
    {
        internal NativeMethods.AuthorizationCodeChallenge self;
        private int disposed_;

        internal AuthorizationCodeChallenge(NativeMethods.AuthorizationCodeChallenge self,
                                            int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~AuthorizationCodeChallenge() { Dispose(); }

        public AuthorizationCodeChallenge()
        {
            NativeMethods.__Init();

            unsafe
            {
                fixed (NativeMethods.AuthorizationCodeChallenge* self = &this.self)
                {
                    NativeMethods.AuthorizationCodeChallenge.Init(self);
                }
            }

            NativeMethods.__OnPostConstruct(this);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.AuthorizationCodeChallenge* self = &this.self)
                {
                    NativeMethods.AuthorizationCodeChallenge.Drop(self);
                }
            }
        }

        public AuthorizationCodeChallenge(AuthorizationCodeChallenge other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationCodeChallenge));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.AuthorizationCodeChallenge* otherPtr = &other.self)
                {
                    fixed (NativeMethods.AuthorizationCodeChallenge* selfPtr = &self)
                    {
                        NativeMethods.AuthorizationCodeChallenge.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe AuthorizationCodeChallenge(NativeMethods.AuthorizationCodeChallenge* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.AuthorizationCodeChallenge* selfPtr = &self)
                {
                    NativeMethods.AuthorizationCodeChallenge.Clone(selfPtr, otherPtr);
                }
            }
        }

        public AuthenticationCodeChallengeMethod Method()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationCodeChallenge));
            }

            unsafe
            {
                AuthenticationCodeChallengeMethod __returnValue;

                fixed (NativeMethods.AuthorizationCodeChallenge* self = &this.self)
                {
                    __returnValue = NativeMethods.AuthorizationCodeChallenge.Method(self);
                }

                return __returnValue;
            }
        }

        public void SetMethod(AuthenticationCodeChallengeMethod value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationCodeChallenge));
            }

            unsafe
            {
                fixed (NativeMethods.AuthorizationCodeChallenge* self = &this.self)
                {
                    NativeMethods.AuthorizationCodeChallenge.SetMethod(self, value);
                }
            }
        }

        public string Challenge()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationCodeChallenge));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.AuthorizationCodeChallenge* self = &this.self)
                {
                    NativeMethods.AuthorizationCodeChallenge.Challenge(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetChallenge(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationCodeChallenge));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.AuthorizationCodeChallenge* self = &this.self)
                {
                    NativeMethods.AuthorizationCodeChallenge.SetChallenge(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }
    }

    /// <summary>
    ///  Struct that encapsulates both parts of the code verification flow.
    /// </summary>
    public class AuthorizationCodeVerifier : IDisposable
    {
        internal NativeMethods.AuthorizationCodeVerifier self;
        private int disposed_;

        internal AuthorizationCodeVerifier(NativeMethods.AuthorizationCodeVerifier self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~AuthorizationCodeVerifier() { Dispose(); }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.AuthorizationCodeVerifier* self = &this.self)
                {
                    NativeMethods.AuthorizationCodeVerifier.Drop(self);
                }
            }
        }

        public AuthorizationCodeVerifier(AuthorizationCodeVerifier other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationCodeVerifier));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.AuthorizationCodeVerifier* otherPtr = &other.self)
                {
                    fixed (NativeMethods.AuthorizationCodeVerifier* selfPtr = &self)
                    {
                        NativeMethods.AuthorizationCodeVerifier.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe AuthorizationCodeVerifier(NativeMethods.AuthorizationCodeVerifier* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.AuthorizationCodeVerifier* selfPtr = &self)
                {
                    NativeMethods.AuthorizationCodeVerifier.Clone(selfPtr, otherPtr);
                }
            }
        }

        public AuthorizationCodeChallenge Challenge()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationCodeVerifier));
            }

            unsafe
            {
                var __returnValueNative = new NativeMethods.AuthorizationCodeChallenge();
                AuthorizationCodeChallenge? __returnValue = null;

                fixed (NativeMethods.AuthorizationCodeVerifier* self = &this.self)
                {
                    NativeMethods.AuthorizationCodeVerifier.Challenge(self, &__returnValueNative);
                }

                __returnValue = new AuthorizationCodeChallenge(__returnValueNative, 0);
                return __returnValue;
            }
        }

        public void SetChallenge(AuthorizationCodeChallenge value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationCodeVerifier));
            }

            unsafe
            {
                fixed (NativeMethods.AuthorizationCodeChallenge* __valueFixed = &value.self)
                {
                    fixed (NativeMethods.AuthorizationCodeVerifier* self = &this.self)
                    {
                        NativeMethods.AuthorizationCodeVerifier.SetChallenge(self, __valueFixed);
                    }
                }
            }
        }

        public string Verifier()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationCodeVerifier));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.AuthorizationCodeVerifier* self = &this.self)
                {
                    NativeMethods.AuthorizationCodeVerifier.Verifier(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetVerifier(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationCodeVerifier));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.AuthorizationCodeVerifier* self = &this.self)
                {
                    NativeMethods.AuthorizationCodeVerifier.SetVerifier(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }
    }

    /// <summary>
    ///  Arguments to the Client::Authorize function.
    /// </summary>
    public class AuthorizationArgs : IDisposable
    {
        internal NativeMethods.AuthorizationArgs self;
        private int disposed_;

        internal AuthorizationArgs(NativeMethods.AuthorizationArgs self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~AuthorizationArgs() { Dispose(); }

        public AuthorizationArgs()
        {
            NativeMethods.__Init();

            unsafe
            {
                fixed (NativeMethods.AuthorizationArgs* self = &this.self)
                {
                    NativeMethods.AuthorizationArgs.Init(self);
                }
            }

            NativeMethods.__OnPostConstruct(this);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.AuthorizationArgs* self = &this.self)
                {
                    NativeMethods.AuthorizationArgs.Drop(self);
                }
            }
        }

        public AuthorizationArgs(AuthorizationArgs other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationArgs));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.AuthorizationArgs* otherPtr = &other.self)
                {
                    fixed (NativeMethods.AuthorizationArgs* selfPtr = &self)
                    {
                        NativeMethods.AuthorizationArgs.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe AuthorizationArgs(NativeMethods.AuthorizationArgs* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.AuthorizationArgs* selfPtr = &self)
                {
                    NativeMethods.AuthorizationArgs.Clone(selfPtr, otherPtr);
                }
            }
        }

        public ulong ClientId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationArgs));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.AuthorizationArgs* self = &this.self)
                {
                    __returnValue = NativeMethods.AuthorizationArgs.ClientId(self);
                }

                return __returnValue;
            }
        }

        public void SetClientId(ulong value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationArgs));
            }

            unsafe
            {
                fixed (NativeMethods.AuthorizationArgs* self = &this.self)
                {
                    NativeMethods.AuthorizationArgs.SetClientId(self, value);
                }
            }
        }

        public string Scopes()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationArgs));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.AuthorizationArgs* self = &this.self)
                {
                    NativeMethods.AuthorizationArgs.Scopes(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetScopes(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationArgs));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.AuthorizationArgs* self = &this.self)
                {
                    NativeMethods.AuthorizationArgs.SetScopes(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public string? State()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationArgs));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.AuthorizationArgs* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.AuthorizationArgs.State(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetState(string? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationArgs));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned = NativeMethods.__InitNullableStringLocal(
                    __scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.AuthorizationArgs* self = &this.self)
                {
                    NativeMethods.AuthorizationArgs.SetState(self,
                        (value != null ? &__valueSpan : null));
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public string? Nonce()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationArgs));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.AuthorizationArgs* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.AuthorizationArgs.Nonce(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetNonce(string? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationArgs));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned = NativeMethods.__InitNullableStringLocal(
                    __scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.AuthorizationArgs* self = &this.self)
                {
                    NativeMethods.AuthorizationArgs.SetNonce(self,
                        (value != null ? &__valueSpan : null));
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public AuthorizationCodeChallenge? CodeChallenge()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationArgs));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.AuthorizationCodeChallenge();
                AuthorizationCodeChallenge? __returnValue = null;

                fixed (NativeMethods.AuthorizationArgs* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.AuthorizationArgs.CodeChallenge(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new AuthorizationCodeChallenge(__returnValueNative, 0);
                return __returnValue;
            }
        }

        public void SetCodeChallenge(AuthorizationCodeChallenge? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationArgs));
            }

            unsafe
            {
                var __valueLocal = value?.self ?? default;

                fixed (NativeMethods.AuthorizationArgs* self = &this.self)
                {
                    NativeMethods.AuthorizationArgs.SetCodeChallenge(
                        self, (value != null ? &__valueLocal : null));
                }

                if (value != null)
                {
                    value.self = __valueLocal;
                }
            }
        }

        public IntegrationType? IntegrationType()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationArgs));
            }

            unsafe
            {
                bool __returnIsNonNull;
                IntegrationType __returnValue;

                fixed (NativeMethods.AuthorizationArgs* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.AuthorizationArgs.IntegrationType(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                return __returnValue;
            }
        }

        public void SetIntegrationType(IntegrationType? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationArgs));
            }

            unsafe
            {
                var __valueLocal = value ?? default;

                fixed (NativeMethods.AuthorizationArgs* self = &this.self)
                {
                    NativeMethods.AuthorizationArgs.SetIntegrationType(
                        self, (value != null ? &__valueLocal : null));
                }
            }
        }

        public string? CustomSchemeParam()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationArgs));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.AuthorizationArgs* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.AuthorizationArgs.CustomSchemeParam(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetCustomSchemeParam(string? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AuthorizationArgs));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned = NativeMethods.__InitNullableStringLocal(
                    __scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.AuthorizationArgs* self = &this.self)
                {
                    NativeMethods.AuthorizationArgs.SetCustomSchemeParam(
                        self, (value != null ? &__valueSpan : null));
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }
    }

    /// <summary>
    ///  Arguments to the Client::GetTokenFromDevice function.
    /// </summary>
    public class DeviceAuthorizationArgs : IDisposable
    {
        internal NativeMethods.DeviceAuthorizationArgs self;
        private int disposed_;

        internal DeviceAuthorizationArgs(NativeMethods.DeviceAuthorizationArgs self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~DeviceAuthorizationArgs() { Dispose(); }

        public DeviceAuthorizationArgs()
        {
            NativeMethods.__Init();

            unsafe
            {
                fixed (NativeMethods.DeviceAuthorizationArgs* self = &this.self)
                {
                    NativeMethods.DeviceAuthorizationArgs.Init(self);
                }
            }

            NativeMethods.__OnPostConstruct(this);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.DeviceAuthorizationArgs* self = &this.self)
                {
                    NativeMethods.DeviceAuthorizationArgs.Drop(self);
                }
            }
        }

        public DeviceAuthorizationArgs(DeviceAuthorizationArgs other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(DeviceAuthorizationArgs));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.DeviceAuthorizationArgs* otherPtr = &other.self)
                {
                    fixed (NativeMethods.DeviceAuthorizationArgs* selfPtr = &self)
                    {
                        NativeMethods.DeviceAuthorizationArgs.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe DeviceAuthorizationArgs(NativeMethods.DeviceAuthorizationArgs* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.DeviceAuthorizationArgs* selfPtr = &self)
                {
                    NativeMethods.DeviceAuthorizationArgs.Clone(selfPtr, otherPtr);
                }
            }
        }

        public ulong ClientId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(DeviceAuthorizationArgs));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.DeviceAuthorizationArgs* self = &this.self)
                {
                    __returnValue = NativeMethods.DeviceAuthorizationArgs.ClientId(self);
                }

                return __returnValue;
            }
        }

        public void SetClientId(ulong value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(DeviceAuthorizationArgs));
            }

            unsafe
            {
                fixed (NativeMethods.DeviceAuthorizationArgs* self = &this.self)
                {
                    NativeMethods.DeviceAuthorizationArgs.SetClientId(self, value);
                }
            }
        }

        public string Scopes()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(DeviceAuthorizationArgs));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.DeviceAuthorizationArgs* self = &this.self)
                {
                    NativeMethods.DeviceAuthorizationArgs.Scopes(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetScopes(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(DeviceAuthorizationArgs));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.DeviceAuthorizationArgs* self = &this.self)
                {
                    NativeMethods.DeviceAuthorizationArgs.SetScopes(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }
    }

    /// <summary>
    ///  A VoiceStateHandle represents the state of a single participant in a Discord voice call.
    /// </summary>
    /// <remarks>
    ///  The main use case for VoiceStateHandle in the SDK is communicate whether a user has muted or
    ///  defeaned themselves.
    ///
    ///  Handle objects in the SDK hold a reference both to the underlying data, and to the SDK
    ///  instance. Changes to the underlying data will generally be available on existing handles
    ///  objects without having to re-create them. If the SDK instance is destroyed, but you still have
    ///  a reference to a handle object, note that it will return the default value for all method calls
    ///  (ie an empty string for methods that return a string).
    ///
    /// </remarks>
    public class VoiceStateHandle : IDisposable
    {
        internal NativeMethods.VoiceStateHandle self;
        private int disposed_;

        internal VoiceStateHandle(NativeMethods.VoiceStateHandle self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~VoiceStateHandle() { Dispose(); }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.VoiceStateHandle* self = &this.self)
                {
                    NativeMethods.VoiceStateHandle.Drop(self);
                }
            }
        }

        public VoiceStateHandle(VoiceStateHandle other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(VoiceStateHandle));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.VoiceStateHandle* otherPtr = &other.self)
                {
                    fixed (NativeMethods.VoiceStateHandle* selfPtr = &self)
                    {
                        NativeMethods.VoiceStateHandle.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe VoiceStateHandle(NativeMethods.VoiceStateHandle* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.VoiceStateHandle* selfPtr = &self)
                {
                    NativeMethods.VoiceStateHandle.Clone(selfPtr, otherPtr);
                }
            }
        }

        /// <summary>
        ///  Returns true if the given user has deafened themselves so that no one else in the call can
        ///  hear them and so that they do not hear anyone else in the call either.
        /// </summary>
        public bool SelfDeaf()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(VoiceStateHandle));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.VoiceStateHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.VoiceStateHandle.SelfDeaf(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns true if the given user has muted themselves so that no one else in the call can
        ///  hear them.
        /// </summary>
        public bool SelfMute()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(VoiceStateHandle));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.VoiceStateHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.VoiceStateHandle.SelfMute(self);
                }

                return __returnValue;
            }
        }
    }

    /// <summary>
    ///  Settings for the void auto detection threshold for picking up activity from a user's mic.
    /// </summary>
    public class VADThresholdSettings : IDisposable
    {
        internal NativeMethods.VADThresholdSettings self;
        private int disposed_;

        internal VADThresholdSettings(NativeMethods.VADThresholdSettings self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~VADThresholdSettings() { Dispose(); }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.VADThresholdSettings* self = &this.self)
                {
                    NativeMethods.VADThresholdSettings.Drop(self);
                }
            }
        }

        public float VadThreshold()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(VADThresholdSettings));
            }

            unsafe
            {
                float __returnValue;

                fixed (NativeMethods.VADThresholdSettings* self = &this.self)
                {
                    __returnValue = NativeMethods.VADThresholdSettings.VadThreshold(self);
                }

                return __returnValue;
            }
        }

        public void SetVadThreshold(float value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(VADThresholdSettings));
            }

            unsafe
            {
                fixed (NativeMethods.VADThresholdSettings* self = &this.self)
                {
                    NativeMethods.VADThresholdSettings.SetVadThreshold(self, value);
                }
            }
        }

        public bool Automatic()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(VADThresholdSettings));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.VADThresholdSettings* self = &this.self)
                {
                    __returnValue = NativeMethods.VADThresholdSettings.Automatic(self);
                }

                return __returnValue;
            }
        }

        public void SetAutomatic(bool value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(VADThresholdSettings));
            }

            unsafe
            {
                fixed (NativeMethods.VADThresholdSettings* self = &this.self)
                {
                    NativeMethods.VADThresholdSettings.SetAutomatic(self, value);
                }
            }
        }
    }

    /// <summary>
    ///  Class that manages an active voice session in a Lobby.
    /// </summary>
    public class Call : IDisposable
    {
        internal NativeMethods.Call self;
        private int disposed_;

        internal Call(NativeMethods.Call self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~Call() { Dispose(); }

        /// <summary>
        ///  Enum that represents any network errors with the Call.
        /// </summary>
        public enum Error
        {
            None = 0,
            SignalingConnectionFailed = 1,
            SignalingUnexpectedClose = 2,
            VoiceConnectionFailed = 3,
            JoinTimeout = 4,
            Forbidden = 5,
        }

        /// <summary>
        ///  Enum that respresents the state of the Call's network connection.
        /// </summary>
        public enum Status
        {
            Disconnected = 0,
            Joining = 1,
            Connecting = 2,
            SignalingConnected = 3,
            Connected = 4,
            Reconnecting = 5,
            Disconnecting = 6,
        }

        public delegate void OnVoiceStateChanged(ulong userId);

        public delegate void OnParticipantChanged(ulong userId, bool added);

        public delegate void OnSpeakingStatusChanged(ulong userId, bool isPlayingSound);

        public delegate void OnStatusChanged(Status status,
                                             Error error,
                                             int errorDetail);

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.Call* self = &this.self)
                {
                    NativeMethods.Call.Drop(self);
                }
            }
        }

        public Call(Call other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.Call* otherPtr = &other.self)
                {
                    fixed (NativeMethods.Call* selfPtr = &self)
                    {
                        NativeMethods.Call.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe Call(NativeMethods.Call* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.Call* selfPtr = &self)
                {
                    NativeMethods.Call.Clone(selfPtr, otherPtr);
                }
            }
        }

        /// <summary>
        ///  Converts the Error enum to a string.
        /// </summary>
        public static string ErrorToString(Error type)
        {
            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();
                NativeMethods.Call.ErrorToString(type, &__returnValue);
                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns whether the call is configured to use voice auto detection or push to talk for the
        ///  current user.
        /// </summary>
        public AudioModeType GetAudioMode()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                AudioModeType __returnValue;

                fixed (NativeMethods.Call* self = &this.self)
                {
                    __returnValue = NativeMethods.Call.GetAudioMode(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the ID of the lobby with which this call is associated.
        /// </summary>
        public ulong GetChannelId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.Call* self = &this.self)
                {
                    __returnValue = NativeMethods.Call.GetChannelId(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the ID of the lobby with which this call is associated.
        /// </summary>
        public ulong GetGuildId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.Call* self = &this.self)
                {
                    __returnValue = NativeMethods.Call.GetGuildId(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns whether the current user has locally muted the given userId for themselves.
        /// </summary>
        public bool GetLocalMute(ulong userId)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.Call* self = &this.self)
                {
                    __returnValue = NativeMethods.Call.GetLocalMute(self, userId);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns a list of all of the user IDs of the participants in the call.
        /// </summary>
        public ulong[] GetParticipants()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_UInt64Span();

                fixed (NativeMethods.Call* self = &this.self)
                {
                    NativeMethods.Call.GetParticipants(self, &__returnValue);
                }

                var __returnValueSurface =
                    new Span<ulong>(__returnValue.ptr, (int)__returnValue.size).ToArray();
                NativeMethods.Discord_Free(__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns the locally set playout volume of the given userId.
        /// </summary>
        /// <remarks>
        ///  Does not affect the volume of this user for any other connected clients. The range of
        ///  volume is [0, 200], where 100 indicate default audio volume of the playback device.
        ///
        /// </remarks>
        public float GetParticipantVolume(ulong userId)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                float __returnValue;

                fixed (NativeMethods.Call* self = &this.self)
                {
                    __returnValue = NativeMethods.Call.GetParticipantVolume(self, userId);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns whether push to talk is currently active, meaning the user is currently pressing
        ///  their configured push to talk key.
        /// </summary>
        public bool GetPTTActive()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.Call* self = &this.self)
                {
                    __returnValue = NativeMethods.Call.GetPTTActive(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the time that PTT is active after the user releases the PTT key and
        ///  SetPTTActive(false) is called.
        /// </summary>
        public uint GetPTTReleaseDelay()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                uint __returnValue;

                fixed (NativeMethods.Call* self = &this.self)
                {
                    __returnValue = NativeMethods.Call.GetPTTReleaseDelay(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns whether the current user is deafened.
        /// </summary>
        public bool GetSelfDeaf()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.Call* self = &this.self)
                {
                    __returnValue = NativeMethods.Call.GetSelfDeaf(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns whether the current user's microphone is muted.
        /// </summary>
        public bool GetSelfMute()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.Call* self = &this.self)
                {
                    __returnValue = NativeMethods.Call.GetSelfMute(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the current call status.
        /// </summary>
        /// <remarks>
        ///  A call is not ready to be used until the status changes to "Connected".
        ///
        /// </remarks>
        public Status GetStatus()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                Status __returnValue;

                fixed (NativeMethods.Call* self = &this.self)
                {
                    __returnValue = NativeMethods.Call.GetStatus(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the current configuration for void auto detection thresholds. See the description
        ///  of the VADThreshold struct for specifics.
        /// </summary>
        public VADThresholdSettings GetVADThreshold()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                var __returnValueNative = new NativeMethods.VADThresholdSettings();
                VADThresholdSettings? __returnValue = null;

                fixed (NativeMethods.Call* self = &this.self)
                {
                    NativeMethods.Call.GetVADThreshold(self, &__returnValueNative);
                }

                __returnValue = new VADThresholdSettings(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns a reference to the VoiceStateHandle for the user ID of the given call participant.
        /// </summary>
        /// <remarks>
        ///  The VoiceStateHandle allows other users to know if the target user has muted or deafened
        ///  themselves.
        ///
        /// </remarks>
        public VoiceStateHandle? GetVoiceStateHandle(ulong userId)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.VoiceStateHandle();
                VoiceStateHandle? __returnValue = null;

                fixed (NativeMethods.Call* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.Call.GetVoiceStateHandle(self, userId, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new VoiceStateHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Sets whether to use voice auto detection or push to talk for the current user on this call.
        /// </summary>
        /// <remarks>
        ///  If using push to talk you should call SetPTTActive() whenever the user presses their
        ///  confused push to talk key.
        ///
        /// </remarks>
        public void SetAudioMode(AudioModeType audioMode)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                fixed (NativeMethods.Call* self = &this.self)
                {
                    NativeMethods.Call.SetAudioMode(self, audioMode);
                }
            }
        }

        /// <summary>
        ///  Locally mutes the given userId, so that the current user cannot hear them anymore.
        /// </summary>
        /// <remarks>
        ///  Does not affect whether the given user is muted for any other connected clients.
        ///
        /// </remarks>
        public void SetLocalMute(ulong userId, bool mute)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                fixed (NativeMethods.Call* self = &this.self)
                {
                    NativeMethods.Call.SetLocalMute(self, userId, mute);
                }
            }
        }

        /// <summary>
        ///  Sets a callback function to generally be invoked whenever a field on a VoiceStateHandle
        ///  object for a user would have changed.
        /// </summary>
        /// <remarks>
        ///  For example when a user mutes themselves, all other connected clients will invoke the
        ///  VoiceStateChanged callback, because the "self mute" field will be true now. The callback is
        ///  generally not invoked when users join or leave channels.
        ///
        /// </remarks>
        public void SetOnVoiceStateChangedCallback(OnVoiceStateChanged cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Call.OnVoiceStateChanged __cbDelegate =
                    NativeMethods.Call.OnVoiceStateChanged_Handler;

                fixed (NativeMethods.Call* self = &this.self)
                {
                    NativeMethods.Call.SetOnVoiceStateChangedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sets a callback function to be invoked whenever some joins or leaves a voice call.
        /// </summary>
        public void SetParticipantChangedCallback(OnParticipantChanged cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Call.OnParticipantChanged __cbDelegate =
                    NativeMethods.Call.OnParticipantChanged_Handler;

                fixed (NativeMethods.Call* self = &this.self)
                {
                    NativeMethods.Call.SetParticipantChangedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Locally changes the playout volume of the given userId.
        /// </summary>
        /// <remarks>
        ///  Does not affect the volume of this user for any other connected clients. The range of
        ///  volume is [0, 200], where 100 indicate default audio volume of the playback device.
        ///
        /// </remarks>
        public void SetParticipantVolume(ulong userId, float volume)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                fixed (NativeMethods.Call* self = &this.self)
                {
                    NativeMethods.Call.SetParticipantVolume(self, userId, volume);
                }
            }
        }

        /// <summary>
        ///  When push to talk is enabled, this should be called whenever the user pushes or releases
        ///  their configured push to talk key. This key must be configured in the game, the SDK does
        ///  not handle keybinds itself.
        /// </summary>
        public void SetPTTActive(bool active)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                fixed (NativeMethods.Call* self = &this.self)
                {
                    NativeMethods.Call.SetPTTActive(self, active);
                }
            }
        }

        /// <summary>
        ///  If set, extends the time that PTT is active after the user releases the PTT key and
        ///  SetPTTActive(false) is called.
        /// </summary>
        /// <remarks>
        ///  Defaults to no release delay, but we recommend setting to 20ms, which is what Discord uses.
        ///
        /// </remarks>
        public void SetPTTReleaseDelay(uint releaseDelayMs)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                fixed (NativeMethods.Call* self = &this.self)
                {
                    NativeMethods.Call.SetPTTReleaseDelay(self, releaseDelayMs);
                }
            }
        }

        /// <summary>
        ///  Mutes all audio from the currently active call for the current user.
        ///  They will not be able to hear any other participants,
        ///  and no other participants will be able to hear the current user either.
        /// </summary>
        public void SetSelfDeaf(bool deaf)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                fixed (NativeMethods.Call* self = &this.self)
                {
                    NativeMethods.Call.SetSelfDeaf(self, deaf);
                }
            }
        }

        /// <summary>
        ///  Mutes the current user's microphone so that no other participant in their active calls can
        ///  hear them.
        /// </summary>
        public void SetSelfMute(bool mute)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                fixed (NativeMethods.Call* self = &this.self)
                {
                    NativeMethods.Call.SetSelfMute(self, mute);
                }
            }
        }

        /// <summary>
        ///  Sets a callback function to be invoked whenever a user starts or stops speaking and is
        ///  passed in the userId and whether they are currently speaking.
        /// </summary>
        /// <remarks>
        ///  It can be invoked in other cases as well, such as if the priority speaker changes or if the
        ///  user plays a soundboard sound.
        ///
        /// </remarks>
        public void SetSpeakingStatusChangedCallback(OnSpeakingStatusChanged cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Call.OnSpeakingStatusChanged __cbDelegate =
                    NativeMethods.Call.OnSpeakingStatusChanged_Handler;

                fixed (NativeMethods.Call* self = &this.self)
                {
                    NativeMethods.Call.SetSpeakingStatusChangedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sets a callback function to be invoked when the call status changes, such as when it fully
        ///  connects or starts reconnecting.
        /// </summary>
        public void SetStatusChangedCallback(OnStatusChanged cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Call.OnStatusChanged __cbDelegate =
                    NativeMethods.Call.OnStatusChanged_Handler;

                fixed (NativeMethods.Call* self = &this.self)
                {
                    NativeMethods.Call.SetStatusChangedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Customizes the void auto detection thresholds for picking up activity from a user's mic.
        ///  - When automatic is set to True, Discord will automatically detect the appropriate
        ///  threshold to use.
        ///  - When automatic is set to False, the given threshold value will be used. Threshold has a
        ///  range of -100, 0, and defaults to -60.
        /// </summary>
        public void SetVADThreshold(bool automatic, float threshold)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Call));
            }

            unsafe
            {
                fixed (NativeMethods.Call* self = &this.self)
                {
                    NativeMethods.Call.SetVADThreshold(self, automatic, threshold);
                }
            }
        }

        /// <summary>
        ///  Converts the Status enum to a string.
        /// </summary>
        public static string StatusToString(Status type)
        {
            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();
                NativeMethods.Call.StatusToString(type, &__returnValue);
                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }
    }

    /// <summary>
    ///  All messages sent on Discord are done so in a Channel. MessageHandle::ChannelId will contain
    ///  the ID of the channel a message was sent in, and Client::GetChannelHandle will return an
    ///  instance of this class.
    /// </summary>
    /// <remarks>
    ///  Handle objects in the SDK hold a reference both to the underlying data, and to the SDK
    ///  instance. Changes to the underlying data will generally be available on existing handles
    ///  objects without having to re-create them. If the SDK instance is destroyed, but you still have
    ///  a reference to a handle object, note that it will return the default value for all method calls
    ///  (ie an empty string for methods that return a string).
    ///
    /// </remarks>
    public class ChannelHandle : IDisposable
    {
        internal NativeMethods.ChannelHandle self;
        private int disposed_;

        internal ChannelHandle(NativeMethods.ChannelHandle self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~ChannelHandle() { Dispose(); }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.ChannelHandle* self = &this.self)
                {
                    NativeMethods.ChannelHandle.Drop(self);
                }
            }
        }

        public ChannelHandle(ChannelHandle other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ChannelHandle));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.ChannelHandle* otherPtr = &other.self)
                {
                    fixed (NativeMethods.ChannelHandle* selfPtr = &self)
                    {
                        NativeMethods.ChannelHandle.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe ChannelHandle(NativeMethods.ChannelHandle* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.ChannelHandle* selfPtr = &self)
                {
                    NativeMethods.ChannelHandle.Clone(selfPtr, otherPtr);
                }
            }
        }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public Discord.Sdk.CallInfoHandle? GetCallInfoHandle() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(ChannelHandle));
        }
        unsafe {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.CallInfoHandle();
            CallInfoHandle? __returnValue = null;
            fixed(NativeMethods.ChannelHandle* self = &this.self) {
                __returnIsNonNull =
                  NativeMethods.ChannelHandle.GetCallInfoHandle(self, &__returnValueNative);
            }
            if (!__returnIsNonNull) {
                return null;
            }
            __returnValue = new CallInfoHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
#endif
        /// <summary>
        ///  Returns the ID of the channel.
        /// </summary>
        public ulong Id()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ChannelHandle));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.ChannelHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.ChannelHandle.Id(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the name of the channel.
        /// </summary>
        /// <remarks>
        ///  Generally only channels in servers have names, although Discord may generate a display name
        ///  for some channels as well.
        ///
        /// </remarks>
        public string Name()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ChannelHandle));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ChannelHandle* self = &this.self)
                {
                    NativeMethods.ChannelHandle.Name(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  For DMs and GroupDMs, returns the user IDs of the members of the channel.
        ///  For all other channels returns an empty list.
        /// </summary>
        public ulong[] Recipients()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ChannelHandle));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_UInt64Span();

                fixed (NativeMethods.ChannelHandle* self = &this.self)
                {
                    NativeMethods.ChannelHandle.Recipients(self, &__returnValue);
                }

                var __returnValueSurface =
                    new Span<ulong>(__returnValue.ptr, (int)__returnValue.size).ToArray();
                NativeMethods.Discord_Free(__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns the type of the channel.
        /// </summary>
        public ChannelType Type()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ChannelHandle));
            }

            unsafe
            {
                ChannelType __returnValue;

                fixed (NativeMethods.ChannelHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.ChannelHandle.Type(self);
                }

                return __returnValue;
            }
        }
    }

    /// <summary>
    ///  Represents a guild (also knowns as a Discord server), that the current user is a member of,
    ///  that contains channels that can be linked to a lobby.
    /// </summary>
    public class GuildMinimal : IDisposable
    {
        internal NativeMethods.GuildMinimal self;
        private int disposed_;

        internal GuildMinimal(NativeMethods.GuildMinimal self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~GuildMinimal() { Dispose(); }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.GuildMinimal* self = &this.self)
                {
                    NativeMethods.GuildMinimal.Drop(self);
                }
            }
        }

        public GuildMinimal(GuildMinimal other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildMinimal));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.GuildMinimal* otherPtr = &other.self)
                {
                    fixed (NativeMethods.GuildMinimal* selfPtr = &self)
                    {
                        NativeMethods.GuildMinimal.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe GuildMinimal(NativeMethods.GuildMinimal* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.GuildMinimal* selfPtr = &self)
                {
                    NativeMethods.GuildMinimal.Clone(selfPtr, otherPtr);
                }
            }
        }

        public ulong Id()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildMinimal));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.GuildMinimal* self = &this.self)
                {
                    __returnValue = NativeMethods.GuildMinimal.Id(self);
                }

                return __returnValue;
            }
        }

        public void SetId(ulong value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildMinimal));
            }

            unsafe
            {
                fixed (NativeMethods.GuildMinimal* self = &this.self)
                {
                    NativeMethods.GuildMinimal.SetId(self, value);
                }
            }
        }

        public string Name()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildMinimal));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.GuildMinimal* self = &this.self)
                {
                    NativeMethods.GuildMinimal.Name(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetName(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildMinimal));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.GuildMinimal* self = &this.self)
                {
                    NativeMethods.GuildMinimal.SetName(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }
    }

    /// <summary>
    ///  Represents a channel in a guild that the current user is a member of and may be able to be
    ///  linked to a lobby.
    /// </summary>
    public class GuildChannel : IDisposable
    {
        internal NativeMethods.GuildChannel self;
        private int disposed_;

        internal GuildChannel(NativeMethods.GuildChannel self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~GuildChannel() { Dispose(); }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    NativeMethods.GuildChannel.Drop(self);
                }
            }
        }

        public GuildChannel(GuildChannel other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.GuildChannel* otherPtr = &other.self)
                {
                    fixed (NativeMethods.GuildChannel* selfPtr = &self)
                    {
                        NativeMethods.GuildChannel.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe GuildChannel(NativeMethods.GuildChannel* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.GuildChannel* selfPtr = &self)
                {
                    NativeMethods.GuildChannel.Clone(selfPtr, otherPtr);
                }
            }
        }

        public ulong Id()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    __returnValue = NativeMethods.GuildChannel.Id(self);
                }

                return __returnValue;
            }
        }

        public void SetId(ulong value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            unsafe
            {
                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    NativeMethods.GuildChannel.SetId(self, value);
                }
            }
        }

        public string Name()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    NativeMethods.GuildChannel.Name(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetName(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    NativeMethods.GuildChannel.SetName(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public ChannelType Type()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            unsafe
            {
                ChannelType __returnValue;

                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    __returnValue = NativeMethods.GuildChannel.Type(self);
                }

                return __returnValue;
            }
        }

        public void SetType(ChannelType value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            unsafe
            {
                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    NativeMethods.GuildChannel.SetType(self, value);
                }
            }
        }

        public int Position()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            unsafe
            {
                int __returnValue;

                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    __returnValue = NativeMethods.GuildChannel.Position(self);
                }

                return __returnValue;
            }
        }

        public void SetPosition(int value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            unsafe
            {
                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    NativeMethods.GuildChannel.SetPosition(self, value);
                }
            }
        }

        public ulong? ParentId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            unsafe
            {
                bool __returnIsNonNull;
                ulong __returnValue;

                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.GuildChannel.ParentId(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                return __returnValue;
            }
        }

        public void SetParentId(ulong? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            unsafe
            {
                var __valueLocal = value ?? default;

                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    NativeMethods.GuildChannel.SetParentId(self,
                        (value != null ? &__valueLocal : null));
                }
            }
        }

        public bool IsLinkable()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    __returnValue = NativeMethods.GuildChannel.IsLinkable(self);
                }

                return __returnValue;
            }
        }

        public void SetIsLinkable(bool value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            unsafe
            {
                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    NativeMethods.GuildChannel.SetIsLinkable(self, value);
                }
            }
        }

        public bool IsViewableAndWriteableByAllMembers()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    __returnValue = NativeMethods.GuildChannel.IsViewableAndWriteableByAllMembers(self);
                }

                return __returnValue;
            }
        }

        public void SetIsViewableAndWriteableByAllMembers(bool value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            unsafe
            {
                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    NativeMethods.GuildChannel.SetIsViewableAndWriteableByAllMembers(self, value);
                }
            }
        }

        public LinkedLobby? LinkedLobby()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.LinkedLobby();
                LinkedLobby? __returnValue = null;

                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.GuildChannel.LinkedLobby(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new LinkedLobby(__returnValueNative, 0);
                return __returnValue;
            }
        }

        public void SetLinkedLobby(LinkedLobby? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(GuildChannel));
            }

            unsafe
            {
                var __valueLocal = value?.self ?? default;

                fixed (NativeMethods.GuildChannel* self = &this.self)
                {
                    NativeMethods.GuildChannel.SetLinkedLobby(self,
                        (value != null ? &__valueLocal : null));
                }

                if (value != null)
                {
                    value.self = __valueLocal;
                }
            }
        }
    }

    /// <summary>
    ///  Struct that stores information about the lobby linked to a channel.
    /// </summary>
    public class LinkedLobby : IDisposable
    {
        internal NativeMethods.LinkedLobby self;
        private int disposed_;

        internal LinkedLobby(NativeMethods.LinkedLobby self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~LinkedLobby() { Dispose(); }

        public LinkedLobby()
        {
            NativeMethods.__Init();

            unsafe
            {
                fixed (NativeMethods.LinkedLobby* self = &this.self)
                {
                    NativeMethods.LinkedLobby.Init(self);
                }
            }

            NativeMethods.__OnPostConstruct(this);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.LinkedLobby* self = &this.self)
                {
                    NativeMethods.LinkedLobby.Drop(self);
                }
            }
        }

        public LinkedLobby(LinkedLobby other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LinkedLobby));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.LinkedLobby* otherPtr = &other.self)
                {
                    fixed (NativeMethods.LinkedLobby* selfPtr = &self)
                    {
                        NativeMethods.LinkedLobby.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe LinkedLobby(NativeMethods.LinkedLobby* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.LinkedLobby* selfPtr = &self)
                {
                    NativeMethods.LinkedLobby.Clone(selfPtr, otherPtr);
                }
            }
        }

        public ulong ApplicationId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LinkedLobby));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.LinkedLobby* self = &this.self)
                {
                    __returnValue = NativeMethods.LinkedLobby.ApplicationId(self);
                }

                return __returnValue;
            }
        }

        public void SetApplicationId(ulong value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LinkedLobby));
            }

            unsafe
            {
                fixed (NativeMethods.LinkedLobby* self = &this.self)
                {
                    NativeMethods.LinkedLobby.SetApplicationId(self, value);
                }
            }
        }

        public ulong LobbyId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LinkedLobby));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.LinkedLobby* self = &this.self)
                {
                    __returnValue = NativeMethods.LinkedLobby.LobbyId(self);
                }

                return __returnValue;
            }
        }

        public void SetLobbyId(ulong value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LinkedLobby));
            }

            unsafe
            {
                fixed (NativeMethods.LinkedLobby* self = &this.self)
                {
                    NativeMethods.LinkedLobby.SetLobbyId(self, value);
                }
            }
        }
    }

    /// <summary>
    ///  Struct that stores information about the channel that a lobby is linked to.
    /// </summary>
    public class LinkedChannel : IDisposable
    {
        internal NativeMethods.LinkedChannel self;
        private int disposed_;

        internal LinkedChannel(NativeMethods.LinkedChannel self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~LinkedChannel() { Dispose(); }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.LinkedChannel* self = &this.self)
                {
                    NativeMethods.LinkedChannel.Drop(self);
                }
            }
        }

        public LinkedChannel(LinkedChannel other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LinkedChannel));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.LinkedChannel* otherPtr = &other.self)
                {
                    fixed (NativeMethods.LinkedChannel* selfPtr = &self)
                    {
                        NativeMethods.LinkedChannel.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe LinkedChannel(NativeMethods.LinkedChannel* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.LinkedChannel* selfPtr = &self)
                {
                    NativeMethods.LinkedChannel.Clone(selfPtr, otherPtr);
                }
            }
        }

        public ulong Id()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LinkedChannel));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.LinkedChannel* self = &this.self)
                {
                    __returnValue = NativeMethods.LinkedChannel.Id(self);
                }

                return __returnValue;
            }
        }

        public void SetId(ulong value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LinkedChannel));
            }

            unsafe
            {
                fixed (NativeMethods.LinkedChannel* self = &this.self)
                {
                    NativeMethods.LinkedChannel.SetId(self, value);
                }
            }
        }

        public string Name()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LinkedChannel));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.LinkedChannel* self = &this.self)
                {
                    NativeMethods.LinkedChannel.Name(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetName(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LinkedChannel));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.LinkedChannel* self = &this.self)
                {
                    NativeMethods.LinkedChannel.SetName(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public ulong GuildId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LinkedChannel));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.LinkedChannel* self = &this.self)
                {
                    __returnValue = NativeMethods.LinkedChannel.GuildId(self);
                }

                return __returnValue;
            }
        }

        public void SetGuildId(ulong value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LinkedChannel));
            }

            unsafe
            {
                fixed (NativeMethods.LinkedChannel* self = &this.self)
                {
                    NativeMethods.LinkedChannel.SetGuildId(self, value);
                }
            }
        }
    }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
public class GuildMemberHandle : IDisposable {
    internal NativeMethods.GuildMemberHandle self;
    private int disposed_;

    internal GuildMemberHandle(NativeMethods.GuildMemberHandle self, int disposed) {
        this.self = self;
        this.disposed_ = disposed;
    }

    ~GuildMemberHandle() { Dispose(); }

    public void Dispose() {
        if (Interlocked.Exchange(ref disposed_, 1) != 0) {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe {
            fixed(NativeMethods.GuildMemberHandle* self = &this.self) {
                NativeMethods.GuildMemberHandle.Drop(self);
            }
        }
    }

    public GuildMemberHandle(GuildMemberHandle other) {
        if (other == null) {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildMemberHandle));
        }
        if (other.disposed_ != 0) {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe {
            fixed(NativeMethods.GuildMemberHandle* otherPtr = &other.self) {
                fixed(NativeMethods.GuildMemberHandle* selfPtr = &self) {
                    NativeMethods.GuildMemberHandle.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe GuildMemberHandle(NativeMethods.GuildMemberHandle* otherPtr) {
        unsafe {
            fixed(NativeMethods.GuildMemberHandle* selfPtr = &self) {
                NativeMethods.GuildMemberHandle.Clone(selfPtr, otherPtr);
            }
        }
    }
    public string? Avatar() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildMemberHandle));
        }
        unsafe {
            bool __returnIsNonNull;
            var __returnValue = new NativeMethods.Discord_String();
            fixed(NativeMethods.GuildMemberHandle* self = &this.self) {
                __returnIsNonNull = NativeMethods.GuildMemberHandle.Avatar(self, &__returnValue);
            }
            if (!__returnIsNonNull) {
                return null;
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free((void*)__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public string? AvatarUrl() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildMemberHandle));
        }
        unsafe {
            bool __returnIsNonNull;
            var __returnValue = new NativeMethods.Discord_String();
            fixed(NativeMethods.GuildMemberHandle* self = &this.self) {
                __returnIsNonNull = NativeMethods.GuildMemberHandle.AvatarUrl(self, &__returnValue);
            }
            if (!__returnIsNonNull) {
                return null;
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free((void*)__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public bool Deaf() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildMemberHandle));
        }
        unsafe {
            bool __returnValue;
            fixed(NativeMethods.GuildMemberHandle* self = &this.self) {
                __returnValue = NativeMethods.GuildMemberHandle.Deaf(self);
            }
            return __returnValue;
        }
    }
    public bool IsGuest() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildMemberHandle));
        }
        unsafe {
            bool __returnValue;
            fixed(NativeMethods.GuildMemberHandle* self = &this.self) {
                __returnValue = NativeMethods.GuildMemberHandle.IsGuest(self);
            }
            return __returnValue;
        }
    }
    public bool Mute() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildMemberHandle));
        }
        unsafe {
            bool __returnValue;
            fixed(NativeMethods.GuildMemberHandle* self = &this.self) {
                __returnValue = NativeMethods.GuildMemberHandle.Mute(self);
            }
            return __returnValue;
        }
    }
    public string? NickName() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildMemberHandle));
        }
        unsafe {
            bool __returnIsNonNull;
            var __returnValue = new NativeMethods.Discord_String();
            fixed(NativeMethods.GuildMemberHandle* self = &this.self) {
                __returnIsNonNull = NativeMethods.GuildMemberHandle.NickName(self, &__returnValue);
            }
            if (!__returnIsNonNull) {
                return null;
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free((void*)__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public bool SelfDeaf() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildMemberHandle));
        }
        unsafe {
            bool __returnValue;
            fixed(NativeMethods.GuildMemberHandle* self = &this.self) {
                __returnValue = NativeMethods.GuildMemberHandle.SelfDeaf(self);
            }
            return __returnValue;
        }
    }
    public bool SelfMute() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildMemberHandle));
        }
        unsafe {
            bool __returnValue;
            fixed(NativeMethods.GuildMemberHandle* self = &this.self) {
                __returnValue = NativeMethods.GuildMemberHandle.SelfMute(self);
            }
            return __returnValue;
        }
    }
}
#endif
#if DEVELOPMENT_BUILD || UNITY_EDITOR
public class GuildHandle : IDisposable {
    internal NativeMethods.GuildHandle self;
    private int disposed_;

    internal GuildHandle(NativeMethods.GuildHandle self, int disposed) {
        this.self = self;
        this.disposed_ = disposed;
    }

    ~GuildHandle() { Dispose(); }

    public void Dispose() {
        if (Interlocked.Exchange(ref disposed_, 1) != 0) {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe {
            fixed(NativeMethods.GuildHandle* self = &this.self) {
                NativeMethods.GuildHandle.Drop(self);
            }
        }
    }

    public GuildHandle(GuildHandle other) {
        if (other == null) {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildHandle));
        }
        if (other.disposed_ != 0) {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe {
            fixed(NativeMethods.GuildHandle* otherPtr = &other.self) {
                fixed(NativeMethods.GuildHandle* selfPtr = &self) {
                    NativeMethods.GuildHandle.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe GuildHandle(NativeMethods.GuildHandle* otherPtr) {
        unsafe {
            fixed(NativeMethods.GuildHandle* selfPtr = &self) {
                NativeMethods.GuildHandle.Clone(selfPtr, otherPtr);
            }
        }
    }
    public ulong[] ChannelIds() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildHandle));
        }
        unsafe {
            var __returnValue = new NativeMethods.Discord_UInt64Span();
            fixed(NativeMethods.GuildHandle* self = &this.self) {
                NativeMethods.GuildHandle.ChannelIds(self, &__returnValue);
            }
            var __returnValueSurface =
              new Span<ulong>(__returnValue.ptr, (int)__returnValue.size).ToArray();
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public Discord.Sdk.GuildMemberHandle? GetGuildMemberHandle(ulong userId) {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildHandle));
        }
        unsafe {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.GuildMemberHandle();
            GuildMemberHandle? __returnValue = null;
            fixed(NativeMethods.GuildHandle* self = &this.self) {
                __returnIsNonNull = NativeMethods.GuildHandle.GetGuildMemberHandle(
                  self, userId, &__returnValueNative);
            }
            if (!__returnIsNonNull) {
                return null;
            }
            __returnValue = new GuildMemberHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
    public ulong[] GuildMemberIds() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildHandle));
        }
        unsafe {
            var __returnValue = new NativeMethods.Discord_UInt64Span();
            fixed(NativeMethods.GuildHandle* self = &this.self) {
                NativeMethods.GuildHandle.GuildMemberIds(self, &__returnValue);
            }
            var __returnValueSurface =
              new Span<ulong>(__returnValue.ptr, (int)__returnValue.size).ToArray();
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public string? IconUrl() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildHandle));
        }
        unsafe {
            bool __returnIsNonNull;
            var __returnValue = new NativeMethods.Discord_String();
            fixed(NativeMethods.GuildHandle* self = &this.self) {
                __returnIsNonNull = NativeMethods.GuildHandle.IconUrl(self, &__returnValue);
            }
            if (!__returnIsNonNull) {
                return null;
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free((void*)__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public ulong Id() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildHandle));
        }
        unsafe {
            ulong __returnValue;
            fixed(NativeMethods.GuildHandle* self = &this.self) {
                __returnValue = NativeMethods.GuildHandle.Id(self);
            }
            return __returnValue;
        }
    }
    public string Name() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildHandle));
        }
        unsafe {
            var __returnValue = new NativeMethods.Discord_String();
            fixed(NativeMethods.GuildHandle* self = &this.self) {
                NativeMethods.GuildHandle.Name(self, &__returnValue);
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free((void*)__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public uint PremiumTier() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildHandle));
        }
        unsafe {
            uint __returnValue;
            fixed(NativeMethods.GuildHandle* self = &this.self) {
                __returnValue = NativeMethods.GuildHandle.PremiumTier(self);
            }
            return __returnValue;
        }
    }
    public bool Unavailable() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(GuildHandle));
        }
        unsafe {
            bool __returnValue;
            fixed(NativeMethods.GuildHandle* self = &this.self) {
                __returnValue = NativeMethods.GuildHandle.Unavailable(self);
            }
            return __returnValue;
        }
    }
}
#endif
    /// <summary>
    ///  A RelationshipHandle represents the relationship between the current user and a target user on
    ///  Discord. Relationships include friends, blocked users, and friend invites.
    /// </summary>
    /// <remarks>
    ///  The SDK supports two types of relationships:
    ///  - Discord: These are relationships that persist across games and on the Discord client.
    ///  Both users will be able to see whether each other is online regardless of whether they are in
    ///  the same game or not.
    ///  - Game: These are per-game relationships and do not carry over to other games. The two users
    ///  will only be able to see if the other is online if they are playing a game in which they are
    ///  friends.
    ///
    ///  If someone is a game friend they can later choose to "upgrade" to a full Discord friend. In
    ///  this case, the user has two relationships at the same time, which is why there are two
    ///  different type fields on RelationshipHandle. In this example, their
    ///  RelationshipHandle::DiscordRelationshipType would be set to RelationshipType::PendingIncoming
    ///  or RelationshipType::PendingOutgoing (based on whether they are receiving or sending the invite
    ///  respectively), and their RelationshipHandle::GameRelationshipType would remain as
    ///  RelationshipType::Friend.
    ///
    ///  When a user blocks another user, it is always stored on the
    ///  RelationshipHandle::DiscordRelationshipType field, and will persist across games. It is not
    ///  possible to block a user in only one game.
    ///
    ///  See the @ref friends documentation for more information.
    ///
    ///  Note: While the SDK allows you to manage a user's relationships, you should never take an
    ///  action without their explicit consent. You should not automatically send or accept friend
    ///  requests. Only invoke APIs to manage relationships in response to a user action such as
    ///  clicking a "Send Friend Request" button.
    ///
    ///  Handle objects in the SDK hold a reference both to the underlying data, and to the SDK
    ///  instance. Changes to the underlying data will generally be available on existing handles
    ///  objects without having to re-create them. If the SDK instance is destroyed, but you still have
    ///  a reference to a handle object, note that it will return the default value for all method calls
    ///  (ie an empty string for methods that return a string).
    ///
    /// </remarks>
    public class RelationshipHandle : IDisposable
    {
        internal NativeMethods.RelationshipHandle self;
        private int disposed_;

        internal RelationshipHandle(NativeMethods.RelationshipHandle self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~RelationshipHandle() { Dispose(); }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.RelationshipHandle* self = &this.self)
                {
                    NativeMethods.RelationshipHandle.Drop(self);
                }
            }
        }

        public RelationshipHandle(RelationshipHandle other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(RelationshipHandle));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.RelationshipHandle* otherPtr = &other.self)
                {
                    fixed (NativeMethods.RelationshipHandle* selfPtr = &self)
                    {
                        NativeMethods.RelationshipHandle.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe RelationshipHandle(NativeMethods.RelationshipHandle* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.RelationshipHandle* selfPtr = &self)
                {
                    NativeMethods.RelationshipHandle.Clone(selfPtr, otherPtr);
                }
            }
        }

        /// <summary>
        ///  Returns the type of the Discord relationship.
        /// </summary>
        public RelationshipType DiscordRelationshipType()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(RelationshipHandle));
            }

            unsafe
            {
                RelationshipType __returnValue;

                fixed (NativeMethods.RelationshipHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.RelationshipHandle.DiscordRelationshipType(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the type of the Game relationship.
        /// </summary>
        public RelationshipType GameRelationshipType()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(RelationshipHandle));
            }

            unsafe
            {
                RelationshipType __returnValue;

                fixed (NativeMethods.RelationshipHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.RelationshipHandle.GameRelationshipType(self);
                }

                return __returnValue;
            }
        }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    /// <summary>
    ///  Returns true if the target user has played the game. Note, for Discord friends, this
    ///  value is only correctly reflected if the user is seen as online during the SDK's session.
    ///  For game friends, this value is always true.
    /// </summary>
    public bool HasPlayedGame() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(RelationshipHandle));
        }
        unsafe {
            bool __returnValue;
            fixed(NativeMethods.RelationshipHandle* self = &this.self) {
                __returnValue = NativeMethods.RelationshipHandle.HasPlayedGame(self);
            }
            return __returnValue;
        }
    }
#endif
        /// <summary>
        ///  Returns the ID of the target user in this relationship.
        /// </summary>
        public ulong Id()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(RelationshipHandle));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.RelationshipHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.RelationshipHandle.Id(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns whether this relationship is a spam request.
        /// </summary>
        public bool IsSpamRequest()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(RelationshipHandle));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.RelationshipHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.RelationshipHandle.IsSpamRequest(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns a handle to the target user in this relationship, if one is available.
        ///  This would be the user with the same ID as the one returned by the Id() method.
        /// </summary>
        public UserHandle? User()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(RelationshipHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.UserHandle();
                UserHandle? __returnValue = null;

                fixed (NativeMethods.RelationshipHandle* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.RelationshipHandle.User(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new UserHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }
    }

    /// <summary>
    ///  A UserApplicationProfileHandle represents a profile from an external identity provider, such as
    ///  Steam or Epic Online Services.
    /// </summary>
    /// <remarks>
    ///  Handle objects in the SDK hold a reference both to the underlying data, and to the SDK
    ///  instance. Changes to the underlying data will generally be available on existing handles
    ///  objects without having to re-create them. If the SDK instance is destroyed, but you still have
    ///  a reference to a handle object, note that it will return the default value for all method calls
    ///  (ie an empty string for methods that return a string).
    ///
    /// </remarks>
    public class UserApplicationProfileHandle : IDisposable
    {
        internal NativeMethods.UserApplicationProfileHandle self;
        private int disposed_;

        internal UserApplicationProfileHandle(NativeMethods.UserApplicationProfileHandle self,
                                              int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~UserApplicationProfileHandle() { Dispose(); }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.UserApplicationProfileHandle* self = &this.self)
                {
                    NativeMethods.UserApplicationProfileHandle.Drop(self);
                }
            }
        }

        public UserApplicationProfileHandle(UserApplicationProfileHandle other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserApplicationProfileHandle));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.UserApplicationProfileHandle* otherPtr = &other.self)
                {
                    fixed (NativeMethods.UserApplicationProfileHandle* selfPtr = &self)
                    {
                        NativeMethods.UserApplicationProfileHandle.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe
            UserApplicationProfileHandle(NativeMethods.UserApplicationProfileHandle* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.UserApplicationProfileHandle* selfPtr = &self)
                {
                    NativeMethods.UserApplicationProfileHandle.Clone(selfPtr, otherPtr);
                }
            }
        }

        /// <summary>
        ///  Returns the user's in-game avatar hash.
        /// </summary>
        public string AvatarHash()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserApplicationProfileHandle));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.UserApplicationProfileHandle* self = &this.self)
                {
                    NativeMethods.UserApplicationProfileHandle.AvatarHash(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns any metadata set by the developer.
        /// </summary>
        public string Metadata()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserApplicationProfileHandle));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.UserApplicationProfileHandle* self = &this.self)
                {
                    NativeMethods.UserApplicationProfileHandle.Metadata(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns the user's external identity provider ID if it exists.
        /// </summary>
        public string? ProviderId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserApplicationProfileHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.UserApplicationProfileHandle* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.UserApplicationProfileHandle.ProviderId(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns the user's external identity provider issued user ID.
        /// </summary>
        public string ProviderIssuedUserId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserApplicationProfileHandle));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.UserApplicationProfileHandle* self = &this.self)
                {
                    NativeMethods.UserApplicationProfileHandle.ProviderIssuedUserId(self,
                        &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns the type of the external identity provider.
        /// </summary>
        public ExternalIdentityProviderType ProviderType()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserApplicationProfileHandle));
            }

            unsafe
            {
                ExternalIdentityProviderType __returnValue;

                fixed (NativeMethods.UserApplicationProfileHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.UserApplicationProfileHandle.ProviderType(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the user's in-game username.
        /// </summary>
        public string Username()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserApplicationProfileHandle));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.UserApplicationProfileHandle* self = &this.self)
                {
                    NativeMethods.UserApplicationProfileHandle.Username(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }
    }

    /// <summary>
    ///  A UserHandle represents a single user on Discord that the SDK knows about and contains basic
    ///  account information for them such as id, name, and avatar, as well as their "status"
    ///  information which includes both whether they are online/offline/etc as well as whether they are
    ///  playing this game.
    /// </summary>
    /// <remarks>
    ///  Handle objects in the SDK hold a reference both to the underlying data, and to the SDK
    ///  instance. Changes to the underlying data will generally be available on existing handles
    ///  objects without having to re-create them. If the SDK instance is destroyed, but you still have
    ///  a reference to a handle object, note that it will return the default value for all method calls
    ///  (ie an empty string for methods that return a string).
    ///
    /// </remarks>
    public class UserHandle : IDisposable
    {
        internal NativeMethods.UserHandle self;
        private int disposed_;

        internal UserHandle(NativeMethods.UserHandle self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~UserHandle() { Dispose(); }

        /// <summary>
        ///  The desired type of avatar url to generate for a User.
        /// </summary>
        public enum AvatarType
        {
            Gif = 0,
            Webp = 1,
            Png = 2,
            Jpeg = 3,
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.UserHandle* self = &this.self)
                {
                    NativeMethods.UserHandle.Drop(self);
                }
            }
        }

        public UserHandle(UserHandle other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserHandle));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.UserHandle* otherPtr = &other.self)
                {
                    fixed (NativeMethods.UserHandle* selfPtr = &self)
                    {
                        NativeMethods.UserHandle.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe UserHandle(NativeMethods.UserHandle* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.UserHandle* selfPtr = &self)
                {
                    NativeMethods.UserHandle.Clone(selfPtr, otherPtr);
                }
            }
        }

        /// <summary>
        ///  Returns the hash of the user's Discord profile avatar, if one is set.
        /// </summary>
        public string? Avatar()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.UserHandle* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.UserHandle.Avatar(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Converts the AvatarType enum to a string.
        /// </summary>
        public static string AvatarTypeToString(AvatarType type)
        {
            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();
                NativeMethods.UserHandle.AvatarTypeToString(type, &__returnValue);
                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns a CDN url to the user's Discord profile avatar.
        /// </summary>
        /// <remarks>
        ///  If the user does not have an avatar set, a url to one of Discord's default avatars is
        ///  returned instead.
        ///
        /// </remarks>
        public string AvatarUrl(AvatarType animatedType,
                                AvatarType staticType)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserHandle));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.UserHandle* self = &this.self)
                {
                    NativeMethods.UserHandle.AvatarUrl(self, animatedType, staticType, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns the user's preferred name, if one is set, otherwise returns their unique username.
        /// </summary>
        public string DisplayName()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserHandle));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.UserHandle* self = &this.self)
                {
                    NativeMethods.UserHandle.DisplayName(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns the user's rich presence activity that is associated with the current game, if one
        ///  is set.
        /// </summary>
        /// <remarks>
        ///  On Discord, users can have multiple rich presence activities at once, but the SDK will only
        ///  expose the activity that is associated with your game. You can use this to know about the
        ///  party the user is in, if any, and what the user is doing in the game.
        ///
        ///  For more information see the Activity class and check out
        ///  https://discord.com/developers/docs/rich-presence/overview
        ///
        /// </remarks>
        public Activity? GameActivity()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.Activity();
                Activity? __returnValue = null;

                fixed (NativeMethods.UserHandle* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.UserHandle.GameActivity(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new Activity(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the preferred display name of this user, if one is set.
        /// </summary>
        /// <remarks>
        ///  Discord's public API refers to this as a "global name" instead of "display name".
        ///
        ///  Discord users can set their preferred name to almost any string.
        ///
        ///  For more information about usernames on Discord, see:
        ///  https://discord.com/developers/docs/resources/user
        ///
        /// </remarks>
        public string? GlobalName()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.UserHandle* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.UserHandle.GlobalName(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns the ID of this user.
        /// </summary>
        /// <remarks>
        ///  If this returns 0 then the UserHandle is no longer valid.
        ///
        /// </remarks>
        public ulong Id()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserHandle));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.UserHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.UserHandle.Id(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns true if this user is a provisional account.
        /// </summary>
        public bool IsProvisional()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserHandle));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.UserHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.UserHandle.IsProvisional(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns a reference to the RelationshipHandle between the currently authenticated user and
        ///  this user, if any.
        /// </summary>
        public RelationshipHandle Relationship()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserHandle));
            }

            unsafe
            {
                var __returnValueNative = new NativeMethods.RelationshipHandle();
                RelationshipHandle? __returnValue = null;

                fixed (NativeMethods.UserHandle* self = &this.self)
                {
                    NativeMethods.UserHandle.Relationship(self, &__returnValueNative);
                }

                __returnValue = new RelationshipHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the user's online/offline/idle status.
        /// </summary>
        public StatusType Status()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserHandle));
            }

            unsafe
            {
                StatusType __returnValue;

                fixed (NativeMethods.UserHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.UserHandle.Status(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns a list of UserApplicationProfileHandles for this user. Currently, a user can only
        ///  have a single profile per application, so this list will always contain at most one
        ///  UserApplicationProfileHandle.
        /// </summary>
        public UserApplicationProfileHandle[] UserApplicationProfiles()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserHandle));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_UserApplicationProfileHandleSpan();

                fixed (NativeMethods.UserHandle* self = &this.self)
                {
                    NativeMethods.UserHandle.UserApplicationProfiles(self, &__returnValue);
                }

                var __returnValueSurface =
                    new UserApplicationProfileHandle[(int)__returnValue.size];

                for (int __i = 0; __i < (int)__returnValue.size; __i++)
                {
                    __returnValueSurface[__i] =
                        new UserApplicationProfileHandle(__returnValue.ptr[__i], 0);
                }

                NativeMethods.Discord_Free(__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns the globally unique username of this user.
        /// </summary>
        /// <remarks>
        ///  For provisional accounts this is an auto-generated string.
        ///
        ///  For more information about usernames on Discord, see:
        ///  https://discord.com/developers/docs/resources/user
        ///
        /// </remarks>
        public string Username()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserHandle));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.UserHandle* self = &this.self)
                {
                    NativeMethods.UserHandle.Username(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }
    }

    /// <summary>
    ///  A LobbyMemberHandle represents the state of a single user in a Lobby.
    /// </summary>
    /// <remarks>
    ///  The SDK separates lobby membership into two concepts:
    ///  1. Has the user been added to the lobby by the game developer?
    ///  If the LobbyMemberHandle exists for a user/lobby pair, then the user has been added to the
    ///  lobby.
    ///  2. Does the user have an active game session that is connected to the lobby and will receive
    ///  any lobby messages? It is possible for a game developer to add a user to a lobby while they are
    ///  offline. Also users may temporarily disconnect and rejoin later. So the `Connected` boolean
    ///  tells you whether the user is actively connected to the lobby.
    ///
    ///  Handle objects in the SDK hold a reference both to the underlying data, and to the SDK
    ///  instance. Changes to the underlying data will generally be available on existing handles
    ///  objects without having to re-create them. If the SDK instance is destroyed, but you still have
    ///  a reference to a handle object, note that it will return the default value for all method calls
    ///  (ie an empty string for methods that return a string).
    ///
    /// </remarks>
    public class LobbyMemberHandle : IDisposable
    {
        internal NativeMethods.LobbyMemberHandle self;
        private int disposed_;

        internal LobbyMemberHandle(NativeMethods.LobbyMemberHandle self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~LobbyMemberHandle() { Dispose(); }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.LobbyMemberHandle* self = &this.self)
                {
                    NativeMethods.LobbyMemberHandle.Drop(self);
                }
            }
        }

        public LobbyMemberHandle(LobbyMemberHandle other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LobbyMemberHandle));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.LobbyMemberHandle* otherPtr = &other.self)
                {
                    fixed (NativeMethods.LobbyMemberHandle* selfPtr = &self)
                    {
                        NativeMethods.LobbyMemberHandle.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe LobbyMemberHandle(NativeMethods.LobbyMemberHandle* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.LobbyMemberHandle* selfPtr = &self)
                {
                    NativeMethods.LobbyMemberHandle.Clone(selfPtr, otherPtr);
                }
            }
        }

        /// <summary>
        ///  Returns true if the user is allowed to link a channel to this lobby.
        /// </summary>
        /// <remarks>
        ///  Under the hood this checks if the LobbyMemberFlags::CanLinkLobby flag is set.
        ///  This flag can only be set via the server API, add_lobby_member
        ///  The use case for this is for games that want to restrict a lobby so that only the
        ///  clan/guild/group leader is allowed to manage the linked channel for the lobby.
        ///
        /// </remarks>
        public bool CanLinkLobby()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LobbyMemberHandle));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.LobbyMemberHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.LobbyMemberHandle.CanLinkLobby(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns true if the user is currently connected to the lobby.
        /// </summary>
        public bool Connected()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LobbyMemberHandle));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.LobbyMemberHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.LobbyMemberHandle.Connected(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  The user id of the lobby member.
        /// </summary>
        public ulong Id()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LobbyMemberHandle));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.LobbyMemberHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.LobbyMemberHandle.Id(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Metadata is a set of string key/value pairs that the game developer can use.
        /// </summary>
        /// <remarks>
        ///  A common use case may be to store the game's internal user ID for this user so that every
        ///  member of a lobby knows the discord user ID and the game's internal user ID mapping for
        ///  each user.
        ///
        /// </remarks>
        public Dictionary<string, string> Metadata()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LobbyMemberHandle));
            }

            unsafe
            {
                var __returnValueNative = new NativeMethods.Discord_Properties();

                fixed (NativeMethods.LobbyMemberHandle* self = &this.self)
                {
                    NativeMethods.LobbyMemberHandle.Metadata(self, &__returnValueNative);
                }

                var __returnValue = new Dictionary<string, string>((int)__returnValueNative.size);

                for (int __i = 0; __i < (int)__returnValueNative.size; __i++)
                {
                    var key = Marshal.PtrToStringUTF8((IntPtr)__returnValueNative.keys[__i].ptr,
                        (int)__returnValueNative.keys[__i].size);
                    var value = Marshal.PtrToStringUTF8((IntPtr)__returnValueNative.values[__i].ptr,
                        (int)__returnValueNative.values[__i].size);
                    __returnValue[key] = value;
                }

                NativeMethods.Discord_FreeProperties(__returnValueNative);
                return __returnValue;
            }
        }

        /// <summary>
        ///  The UserHandle of the lobby member.
        /// </summary>
        public UserHandle? User()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LobbyMemberHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.UserHandle();
                UserHandle? __returnValue = null;

                fixed (NativeMethods.LobbyMemberHandle* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.LobbyMemberHandle.User(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new UserHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }
    }

    /// <summary>
    ///  A LobbyHandle represents a single lobby in the SDK. A lobby can be thought of as
    ///  just an arbitrary, developer-controlled group of users that can communicate with each other.
    /// </summary>
    /// <remarks>
    ///  ## Managing Lobbies
    ///  Lobbies can be managed through a set of [Server
    ///  APIs](https://docs.discord.com/developers/resources/lobby), which allow you to create lobbies,
    ///  add and remove users from lobbies, and delete them.
    ///
    ///  There is also an API to create lobbies without any server side component using the
    ///  Client::CreateOrJoinLobby function, which accepts a game-generated secret and will join the
    ///  user to the lobby associated with that secret, creating it if necessary.
    ///
    ///  NOTE: When using this API the secret will auto-expire in 30 days, at which point the existing
    ///  lobby can no longer be joined, but will still exist. We recommend using this for short term
    ///  lobbies and not permanent lobbies. Use the Server API for more permanent lobbies.
    ///
    ///  Members of a lobby are not automatically removed when they close the game or temporarily
    ///  disconnect. When the SDK connects, it will attempt to re-connect to any lobbies it is currently
    ///  a member of.
    ///
    ///  # Lobby Auto-Deletion
    ///  Lobbies are generally ephemeral and will be auto-deleted if they have been idle (meaning no
    ///  users are actively connected to them) for some amount of time. The default is to auto delete
    ///  after 5 minutes, but this can be customized when creating the lobby. As long as one user is
    ///  connected to the lobby though it will not be auto-deleted (meaning they have the SDK running
    ///  and status is set to Ready). Additionally, lobbies that are linked to a channel on Discord will
    ///  not be auto deleted.
    ///
    ///  You can also use the [Server APIs](https://docs.discord.com/developers/resources/lobby) to
    ///  customize this timeout, it can be raised to as high as 7 days, meaning the lobby only gets
    ///  deleted if no one connects to it for an entire week. This should give a good amount of
    ///  permanence to lobbies when needed, but there may be rare cases where a lobby does need to be
    ///  "rebuilt" if everyone is offline for an extended period.
    ///
    ///  # Membership Limits
    ///  Lobbies may have a maximum of 1,000 members, and each user may be in a maximum of 200 lobbies
    ///  per game.
    ///
    ///  ## Audio
    ///  Lobbies support voice calls. Although a lobby is allowed to have 1,000 members, you should not
    ///  try to start voice calls in lobbies that large. We strongly recommend sticking to around 25
    ///  members or fewer for voice calls.
    ///
    ///  See Client::StartCall for more information on how to start a voice call in a lobby.
    ///
    ///  ## Channel Linking
    ///  Lobbies can be linked to a channel on Discord, which allows messages sent in one place to show
    ///  up in the other. Any lobby can be linked to a channel, but only lobby members with the
    ///  LobbyMemberFlags::CanLinkLobby flag are allowed to a link a lobby. This flag must be set using
    ///  the server APIs, which allows you to ensure that only clan/guild/group leaders can link lobbies
    ///  to Discord channels.
    ///
    ///  To setup a link you'll need to use methods in the Client class to fetch the servers (aka
    ///  guilds) and channels a user is a member of and setup the link. The Client::GetUserGuilds and
    ///  Client::GetGuildChannels methods are used to fetch the servers and channels respectively. You
    ///  can use these to show a UI for the user to pick which server and channel they want to link to.
    ///
    ///  Not all channels are linkable. To be linked:
    ///  - The channel must be a guild text channel
    ///  - The channel may not be marked as NSFW
    ///  - The channel must not be currently linked to a different lobby
    ///  - The user must have the following permissions in the channel in order to link it:
    ///    - Manage Channels
    ///    - View Channel
    ///    - Send Messages
    ///
    ///  ### Linking Private Channels
    ///  Discord is allowing all channels the user has access to in a server to be linked in game, even
    ///  if that channel is private to other members of the server. This means that a user could choose
    ///  to link a private "admins chat" channel (assuming they are an admin) in game if they wanted.
    ///
    ///  It's not really possible for the game to know which users should have access to that channel or
    ///  not though. So in this implementation, every member of a lobby will be able to view all
    ///  messages sent in the linked channel and reply to them. If you are going to allow private
    ///  channels to be linked in game, you must make sure that users are aware that their private
    ///  channel will be viewable by everyone in the lobby!
    ///
    ///  To help you identify which channels are public or private, we have added a
    ///  isViewableAndWriteableByAllMembers boolean which is described more in GuildChannel. You can use
    ///  that to just not allow private channels to be linked, or to know when to show a clear warning,
    ///  it's up to you!
    ///
    ///  ## Misc
    ///  Handle objects in the SDK hold a reference both to the underlying data, and to the SDK
    ///  instance. Changes to the underlying data will generally be available on existing handles
    ///  objects without having to re-create them. If the SDK instance is destroyed, but you still have
    ///  a reference to a handle object, note that it will return the default value for all method calls
    ///  (ie an empty string for methods that return a string).
    ///
    /// </remarks>
    public class LobbyHandle : IDisposable
    {
        internal NativeMethods.LobbyHandle self;
        private int disposed_;

        internal LobbyHandle(NativeMethods.LobbyHandle self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~LobbyHandle() { Dispose(); }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.LobbyHandle* self = &this.self)
                {
                    NativeMethods.LobbyHandle.Drop(self);
                }
            }
        }

        public LobbyHandle(LobbyHandle other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LobbyHandle));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.LobbyHandle* otherPtr = &other.self)
                {
                    fixed (NativeMethods.LobbyHandle* selfPtr = &self)
                    {
                        NativeMethods.LobbyHandle.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe LobbyHandle(NativeMethods.LobbyHandle* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.LobbyHandle* selfPtr = &self)
                {
                    NativeMethods.LobbyHandle.Clone(selfPtr, otherPtr);
                }
            }
        }

        /// <summary>
        ///  Returns a reference to the CallInfoHandle if there is an active voice call in this lobby.
        /// </summary>
        /// <remarks>
        ///  This can allow you to display which lobby members are in voice, even if the current user
        ///  has not yet joined the voice call.
        ///
        /// </remarks>
        public CallInfoHandle? GetCallInfoHandle()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LobbyHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.CallInfoHandle();
                CallInfoHandle? __returnValue = null;

                fixed (NativeMethods.LobbyHandle* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.LobbyHandle.GetCallInfoHandle(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new CallInfoHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns a reference to the LobbyMemberHandle for the given user ID, if they are a member of
        ///  this lobby.
        /// </summary>
        public LobbyMemberHandle? GetLobbyMemberHandle(ulong memberId)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LobbyHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.LobbyMemberHandle();
                LobbyMemberHandle? __returnValue = null;

                fixed (NativeMethods.LobbyHandle* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.LobbyHandle.GetLobbyMemberHandle(
                        self, memberId, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new LobbyMemberHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the id of the lobby.
        /// </summary>
        public ulong Id()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LobbyHandle));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.LobbyHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.LobbyHandle.Id(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns information about the channel linked to this lobby, if any.
        /// </summary>
        public LinkedChannel? LinkedChannel()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LobbyHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.LinkedChannel();
                LinkedChannel? __returnValue = null;

                fixed (NativeMethods.LobbyHandle* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.LobbyHandle.LinkedChannel(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new LinkedChannel(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns a list of the user IDs that are members of this lobby.
        /// </summary>
        public ulong[] LobbyMemberIds()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LobbyHandle));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_UInt64Span();

                fixed (NativeMethods.LobbyHandle* self = &this.self)
                {
                    NativeMethods.LobbyHandle.LobbyMemberIds(self, &__returnValue);
                }

                var __returnValueSurface =
                    new Span<ulong>(__returnValue.ptr, (int)__returnValue.size).ToArray();
                NativeMethods.Discord_Free(__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns a list of the LobbyMemberHandle objects for each member of this lobby.
        /// </summary>
        public LobbyMemberHandle[] LobbyMembers()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LobbyHandle));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_LobbyMemberHandleSpan();

                fixed (NativeMethods.LobbyHandle* self = &this.self)
                {
                    NativeMethods.LobbyHandle.LobbyMembers(self, &__returnValue);
                }

                var __returnValueSurface = new LobbyMemberHandle[(int)__returnValue.size];

                for (int __i = 0; __i < (int)__returnValue.size; __i++)
                {
                    __returnValueSurface[__i] = new LobbyMemberHandle(__returnValue.ptr[__i], 0);
                }

                NativeMethods.Discord_Free(__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns any developer supplied metadata for this lobby.
        /// </summary>
        /// <remarks>
        ///  Metadata is simple string key/value pairs and is a way to associate internal game
        ///  information with the lobby so each lobby member can have easy access to.
        ///
        /// </remarks>
        public Dictionary<string, string> Metadata()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(LobbyHandle));
            }

            unsafe
            {
                var __returnValueNative = new NativeMethods.Discord_Properties();

                fixed (NativeMethods.LobbyHandle* self = &this.self)
                {
                    NativeMethods.LobbyHandle.Metadata(self, &__returnValueNative);
                }

                var __returnValue = new Dictionary<string, string>((int)__returnValueNative.size);

                for (int __i = 0; __i < (int)__returnValueNative.size; __i++)
                {
                    var key = Marshal.PtrToStringUTF8((IntPtr)__returnValueNative.keys[__i].ptr,
                        (int)__returnValueNative.keys[__i].size);
                    var value = Marshal.PtrToStringUTF8((IntPtr)__returnValueNative.values[__i].ptr,
                        (int)__returnValueNative.values[__i].size);
                    __returnValue[key] = value;
                }

                NativeMethods.Discord_FreeProperties(__returnValueNative);
                return __returnValue;
            }
        }
    }

    /// <summary>
    ///  Contains information about non-text content in a message that likely cannot be rendered in game
    ///  such as images, videos, embeds, polls, and more.
    /// </summary>
    public class AdditionalContent : IDisposable
    {
        internal NativeMethods.AdditionalContent self;
        private int disposed_;

        internal AdditionalContent(NativeMethods.AdditionalContent self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~AdditionalContent() { Dispose(); }

        public AdditionalContent()
        {
            NativeMethods.__Init();

            unsafe
            {
                fixed (NativeMethods.AdditionalContent* self = &this.self)
                {
                    NativeMethods.AdditionalContent.Init(self);
                }
            }

            NativeMethods.__OnPostConstruct(this);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.AdditionalContent* self = &this.self)
                {
                    NativeMethods.AdditionalContent.Drop(self);
                }
            }
        }

        public AdditionalContent(AdditionalContent other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AdditionalContent));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.AdditionalContent* otherPtr = &other.self)
                {
                    fixed (NativeMethods.AdditionalContent* selfPtr = &self)
                    {
                        NativeMethods.AdditionalContent.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe AdditionalContent(NativeMethods.AdditionalContent* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.AdditionalContent* selfPtr = &self)
                {
                    NativeMethods.AdditionalContent.Clone(selfPtr, otherPtr);
                }
            }
        }

        /// <summary>
        ///  Compares each field of the AdditionalContent struct for equality.
        /// </summary>
        public bool Equals(AdditionalContent rhs)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AdditionalContent));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.AdditionalContent* __rhsFixed = &rhs.self)
                {
                    fixed (NativeMethods.AdditionalContent* self = &this.self)
                    {
                        __returnValue = NativeMethods.AdditionalContent.Equals(self, __rhsFixed);
                    }
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Converts the AdditionalContentType enum to a string.
        /// </summary>
        public static string TypeToString(AdditionalContentType type)
        {
            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();
                NativeMethods.AdditionalContent.TypeToString(type, &__returnValue);
                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public AdditionalContentType Type()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AdditionalContent));
            }

            unsafe
            {
                AdditionalContentType __returnValue;

                fixed (NativeMethods.AdditionalContent* self = &this.self)
                {
                    __returnValue = NativeMethods.AdditionalContent.Type(self);
                }

                return __returnValue;
            }
        }

        public void SetType(AdditionalContentType value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AdditionalContent));
            }

            unsafe
            {
                fixed (NativeMethods.AdditionalContent* self = &this.self)
                {
                    NativeMethods.AdditionalContent.SetType(self, value);
                }
            }
        }

        public string? Title()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AdditionalContent));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.AdditionalContent* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.AdditionalContent.Title(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetTitle(string? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AdditionalContent));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned = NativeMethods.__InitNullableStringLocal(
                    __scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.AdditionalContent* self = &this.self)
                {
                    NativeMethods.AdditionalContent.SetTitle(self,
                        (value != null ? &__valueSpan : null));
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public byte Count()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AdditionalContent));
            }

            unsafe
            {
                byte __returnValue;

                fixed (NativeMethods.AdditionalContent* self = &this.self)
                {
                    __returnValue = NativeMethods.AdditionalContent.Count(self);
                }

                return __returnValue;
            }
        }

        public void SetCount(byte value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AdditionalContent));
            }

            unsafe
            {
                fixed (NativeMethods.AdditionalContent* self = &this.self)
                {
                    NativeMethods.AdditionalContent.SetCount(self, value);
                }
            }
        }
    }

    /// <summary>
    ///  A MessageHandle represents a single message received by the SDK.
    /// </summary>
    /// <remarks>
    ///  # Chat types
    ///  The SDK supports two types of chat:
    ///  1. 1 on 1 chat between two users
    ///  2. Chat within a lobby
    ///
    ///  You can determine the context a message was sent in with the MessageHandle::Channel and
    ///  ChannelHandle::Type methods. The SDK should only be receiving messages in the following channel
    ///  types:
    ///  - DM
    ///  - Ephemeral DM
    ///  - Lobby
    ///
    ///  # Syncing with Discord
    ///  In some situations messages sent from the SDK will also show up in Discord.
    ///  In general this will happen for:
    ///  - 1 on 1 chat when at least one of the users is a full Discord user
    ///  - Lobby chat when the lobby is linked to a Discord channel
    ///
    ///  Additionally the message must have been sent by a user who is not banned on the Discord side.
    ///
    ///  # Legal disclosures
    ///  As a convenience for game developers, the first time a user sends a message in game, and that
    ///  message will show up on the Discord client, the SDK will inject a "fake" message into the chat,
    ///  that contains a basic English explanation of what is happening to the user. You can identify
    ///  these messages with the MessageHandle::DisclosureType method. We encourage you to customize the
    ///  rendering of these messages, possibly changing the wording, translating them, and making them
    ///  look more "official". You can choose to avoid rendering these as well.
    ///
    ///  # History
    ///  The SDK keeps the 25 most recent messages in each channel in memory, but it does not have
    ///  access to any historical messages sent before the SDK was connected. A MessageHandle will keep
    ///  working though even after the SDK has discarded the message for being too old, you just won't
    ///  be able to create a new MessageHandle objects for that message.
    ///
    ///  # Unrenderable Content
    ///  Messages sent on Discord can contain content that may not be renderable in game, such as
    ///  images, videos, embeds, polls, and more. The game isn't expected to render these, but instead
    ///  show a small notice so the user is aware there is more content and a way to view that content
    ///  on Discord. The MessageHandle::AdditionalContent method will contain data about the non-text
    ///  content in this message.
    ///
    ///  There is also more information about the struct of messages on Discord here:
    ///  https://discord.com/developers/docs/resources/message
    ///
    ///  Note: While the SDK allows you to send messages on behalf of a user, you must only do so in
    ///  response to a user action. You should never automatically send messages.
    ///
    ///  Handle objects in the SDK hold a reference both to the underlying data, and to the SDK
    ///  instance. Changes to the underlying data will generally be available on existing handles
    ///  objects without having to re-create them. If the SDK instance is destroyed, but you still have
    ///  a reference to a handle object, note that it will return the default value for all method calls
    ///  (ie an empty string for methods that return a string).
    ///
    /// </remarks>
    public class MessageHandle : IDisposable
    {
        internal NativeMethods.MessageHandle self;
        private int disposed_;

        internal MessageHandle(NativeMethods.MessageHandle self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~MessageHandle() { Dispose(); }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    NativeMethods.MessageHandle.Drop(self);
                }
            }
        }

        public MessageHandle(MessageHandle other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.MessageHandle* otherPtr = &other.self)
                {
                    fixed (NativeMethods.MessageHandle* selfPtr = &self)
                    {
                        NativeMethods.MessageHandle.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe MessageHandle(NativeMethods.MessageHandle* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.MessageHandle* selfPtr = &self)
                {
                    NativeMethods.MessageHandle.Clone(selfPtr, otherPtr);
                }
            }
        }

        /// <summary>
        ///  If the message contains non-text content, such as images, videos, embeds, polls, etc, this
        ///  method will return information about that content.
        /// </summary>
        public AdditionalContent? AdditionalContent()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.AdditionalContent();
                AdditionalContent? __returnValue = null;

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.MessageHandle.AdditionalContent(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new AdditionalContent(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the application ID associated with this message, if any. You can use
        ///  this to identify if the mesage was sent from another child application in
        ///  your catalog.
        /// </summary>
        /// <remarks>
        ///  Note: Parent / child applications are in limited access and the SentFromGame
        ///  field should be relied on for the common case.
        ///
        /// </remarks>
        public ulong? ApplicationId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                ulong __returnValue;

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.MessageHandle.ApplicationId(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the UserHandle for the author of this message.
        /// </summary>
        public UserHandle? Author()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.UserHandle();
                UserHandle? __returnValue = null;

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.MessageHandle.Author(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new UserHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the user ID of the user who sent this message.
        /// </summary>
        public ulong AuthorId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.MessageHandle.AuthorId(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the ChannelHandle for the channel this message was sent in.
        /// </summary>
        public ChannelHandle? Channel()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.ChannelHandle();
                ChannelHandle? __returnValue = null;

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.MessageHandle.Channel(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new ChannelHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the channel ID this message was sent in.
        /// </summary>
        public ulong ChannelId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.MessageHandle.ChannelId(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the content of this message, if any.
        /// </summary>
        /// <remarks>
        ///  A message can be blank if it was sent from Discord but only contains content such as image
        ///  attachments. Certain types of markup, such as markup for emojis and mentions, will be auto
        ///  replaced with a more human readable form, such as `@username` or `:emoji_name:`.
        ///
        /// </remarks>
        public string Content()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    NativeMethods.MessageHandle.Content(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  If this is an auto-generated message that is explaining some integration behavior to users,
        ///  this method will return the type of disclosure so you can customize it.
        /// </summary>
        public DisclosureTypes? DisclosureType()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                DisclosureTypes __returnValue;

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.MessageHandle.DisclosureType(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  The timestamp in millis since the epoch when the message was most recently edited.
        /// </summary>
        /// <remarks>
        ///  Returns 0 if the message has not been edited yet.
        ///
        /// </remarks>
        public ulong EditedTimestamp()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.MessageHandle.EditedTimestamp(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the ID of this message.
        /// </summary>
        public ulong Id()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.MessageHandle.Id(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the LobbyHandle this message was sent in, if it was sent in a lobby.
        /// </summary>
        public LobbyHandle? Lobby()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.LobbyHandle();
                LobbyHandle? __returnValue = null;

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.MessageHandle.Lobby(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new LobbyHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns any metadata the developer included with this message.
        /// </summary>
        /// <remarks>
        ///  Metadata is just a set of simple string key/value pairs.
        ///  An example use case might be to include a character name so you can customize how a message
        ///  renders in game.
        ///
        /// </remarks>
        public Dictionary<string, string> Metadata()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                var __returnValueNative = new NativeMethods.Discord_Properties();

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    NativeMethods.MessageHandle.Metadata(self, &__returnValueNative);
                }

                var __returnValue = new Dictionary<string, string>((int)__returnValueNative.size);

                for (int __i = 0; __i < (int)__returnValueNative.size; __i++)
                {
                    var key = Marshal.PtrToStringUTF8((IntPtr)__returnValueNative.keys[__i].ptr,
                        (int)__returnValueNative.keys[__i].size);
                    var value = Marshal.PtrToStringUTF8((IntPtr)__returnValueNative.values[__i].ptr,
                        (int)__returnValueNative.values[__i].size);
                    __returnValue[key] = value;
                }

                NativeMethods.Discord_FreeProperties(__returnValueNative);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns any moderation metadata the developer set on this message.
        /// </summary>
        /// <remarks>
        ///  Moderation metadata is just a set of simple string key/value pairs.
        ///  An example use case might be to include a flag that indicates the moderation status of the
        ///  message. Another example would be to include a re-written message that is more appropriate
        ///  for the game's audience.
        ///
        /// </remarks>
        public Dictionary<string, string> ModerationMetadata()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                var __returnValueNative = new NativeMethods.Discord_Properties();

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    NativeMethods.MessageHandle.ModerationMetadata(self, &__returnValueNative);
                }

                var __returnValue = new Dictionary<string, string>((int)__returnValueNative.size);

                for (int __i = 0; __i < (int)__returnValueNative.size; __i++)
                {
                    var key = Marshal.PtrToStringUTF8((IntPtr)__returnValueNative.keys[__i].ptr,
                        (int)__returnValueNative.keys[__i].size);
                    var value = Marshal.PtrToStringUTF8((IntPtr)__returnValueNative.values[__i].ptr,
                        (int)__returnValueNative.values[__i].size);
                    __returnValue[key] = value;
                }

                NativeMethods.Discord_FreeProperties(__returnValueNative);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the content of this message, if any, but without replacing any markup from emojis
        ///  and mentions.
        /// </summary>
        /// <remarks>
        ///  A message can be blank if it was sent from Discord but only contains content such as image
        ///  attachments.
        ///
        /// </remarks>
        public string RawContent()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    NativeMethods.MessageHandle.RawContent(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns the UserHandle for the other participant in a DM, if this message was sent in a DM.
        /// </summary>
        public UserHandle? Recipient()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.UserHandle();
                UserHandle? __returnValue = null;

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.MessageHandle.Recipient(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new UserHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  When this message was sent in a DM or Ephemeral DM, this method will return the ID of the
        ///  other user in that DM.
        /// </summary>
        public ulong RecipientId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.MessageHandle.RecipientId(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns true if this message was sent in-game, otherwise false (i.e. from Discord itself).
        ///  If you are using parent / child applications, this will be true if the message was sent
        ///  from any child application.
        /// </summary>
        public bool SentFromGame()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.MessageHandle.SentFromGame(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  The timestamp in millis since the epoch when the message was sent.
        /// </summary>
        public ulong SentTimestamp()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(MessageHandle));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.MessageHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.MessageHandle.SentTimestamp(self);
                }

                return __returnValue;
            }
        }
    }

    /// <summary>
    ///  Represents a single input or output audio device available to the user.
    /// </summary>
    /// <remarks>
    ///  Discord will initialize the audio engine with the system default input and output devices.
    ///  You can change the device through the Client by passing the id of the desired audio device.
    ///
    /// </remarks>
    public class AudioDevice : IDisposable
    {
        internal NativeMethods.AudioDevice self;
        private int disposed_;

        internal AudioDevice(NativeMethods.AudioDevice self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~AudioDevice() { Dispose(); }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.AudioDevice* self = &this.self)
                {
                    NativeMethods.AudioDevice.Drop(self);
                }
            }
        }

        public AudioDevice(AudioDevice other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AudioDevice));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.AudioDevice* otherPtr = &other.self)
                {
                    fixed (NativeMethods.AudioDevice* selfPtr = &self)
                    {
                        NativeMethods.AudioDevice.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe AudioDevice(NativeMethods.AudioDevice* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.AudioDevice* selfPtr = &self)
                {
                    NativeMethods.AudioDevice.Clone(selfPtr, otherPtr);
                }
            }
        }

        /// <summary>
        ///  Compares the ID of two AudioDevice objects for equality.
        /// </summary>
        public bool Equals(AudioDevice rhs)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AudioDevice));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.AudioDevice* __rhsFixed = &rhs.self)
                {
                    fixed (NativeMethods.AudioDevice* self = &this.self)
                    {
                        __returnValue = NativeMethods.AudioDevice.Equals(self, __rhsFixed);
                    }
                }

                return __returnValue;
            }
        }

        public string Id()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AudioDevice));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.AudioDevice* self = &this.self)
                {
                    NativeMethods.AudioDevice.Id(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetId(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AudioDevice));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.AudioDevice* self = &this.self)
                {
                    NativeMethods.AudioDevice.SetId(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public string Name()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AudioDevice));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.AudioDevice* self = &this.self)
                {
                    NativeMethods.AudioDevice.Name(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetName(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AudioDevice));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.AudioDevice* self = &this.self)
                {
                    NativeMethods.AudioDevice.SetName(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public bool IsDefault()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AudioDevice));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.AudioDevice* self = &this.self)
                {
                    __returnValue = NativeMethods.AudioDevice.IsDefault(self);
                }

                return __returnValue;
            }
        }

        public void SetIsDefault(bool value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(AudioDevice));
            }

            unsafe
            {
                fixed (NativeMethods.AudioDevice* self = &this.self)
                {
                    NativeMethods.AudioDevice.SetIsDefault(self, value);
                }
            }
        }
    }

    /// <summary>
    ///  Represents a summary of a DM conversation with a user.
    /// </summary>
    public class UserMessageSummary : IDisposable
    {
        internal NativeMethods.UserMessageSummary self;
        private int disposed_;

        internal UserMessageSummary(NativeMethods.UserMessageSummary self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~UserMessageSummary() { Dispose(); }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.UserMessageSummary* self = &this.self)
                {
                    NativeMethods.UserMessageSummary.Drop(self);
                }
            }
        }

        public UserMessageSummary(UserMessageSummary other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserMessageSummary));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.UserMessageSummary* otherPtr = &other.self)
                {
                    fixed (NativeMethods.UserMessageSummary* selfPtr = &self)
                    {
                        NativeMethods.UserMessageSummary.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe UserMessageSummary(NativeMethods.UserMessageSummary* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.UserMessageSummary* selfPtr = &self)
                {
                    NativeMethods.UserMessageSummary.Clone(selfPtr, otherPtr);
                }
            }
        }

        /// <summary>
        ///  Returns the ID of the last message sent in the DM conversation.
        /// </summary>
        public ulong LastMessageId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserMessageSummary));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.UserMessageSummary* self = &this.self)
                {
                    __returnValue = NativeMethods.UserMessageSummary.LastMessageId(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the ID of the other user in the DM conversation.
        /// </summary>
        public ulong UserId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(UserMessageSummary));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.UserMessageSummary* self = &this.self)
                {
                    __returnValue = NativeMethods.UserMessageSummary.UserId(self);
                }

                return __returnValue;
            }
        }
    }

    /// <summary>
    ///  Options for creating a new Client instance.
    /// </summary>
    /// <remarks>
    ///  This class may be used to set advanced initialization-time options on Client.
    ///
    /// </remarks>
    public class ClientCreateOptions : IDisposable
    {
        internal NativeMethods.ClientCreateOptions self;
        private int disposed_;

        internal ClientCreateOptions(NativeMethods.ClientCreateOptions self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~ClientCreateOptions() { Dispose(); }

        public ClientCreateOptions()
        {
            NativeMethods.__Init();

            unsafe
            {
                fixed (NativeMethods.ClientCreateOptions* self = &this.self)
                {
                    NativeMethods.ClientCreateOptions.Init(self);
                }
            }

            NativeMethods.__OnPostConstruct(this);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.ClientCreateOptions* self = &this.self)
                {
                    NativeMethods.ClientCreateOptions.Drop(self);
                }
            }
        }

        public ClientCreateOptions(ClientCreateOptions other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientCreateOptions));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.ClientCreateOptions* otherPtr = &other.self)
                {
                    fixed (NativeMethods.ClientCreateOptions* selfPtr = &self)
                    {
                        NativeMethods.ClientCreateOptions.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe ClientCreateOptions(NativeMethods.ClientCreateOptions* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.ClientCreateOptions* selfPtr = &self)
                {
                    NativeMethods.ClientCreateOptions.Clone(selfPtr, otherPtr);
                }
            }
        }

        public string WebBase()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientCreateOptions));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ClientCreateOptions* self = &this.self)
                {
                    NativeMethods.ClientCreateOptions.WebBase(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetWebBase(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientCreateOptions));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ClientCreateOptions* self = &this.self)
                {
                    NativeMethods.ClientCreateOptions.SetWebBase(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public string ApiBase()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientCreateOptions));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();

                fixed (NativeMethods.ClientCreateOptions* self = &this.self)
                {
                    NativeMethods.ClientCreateOptions.ApiBase(self, &__returnValue);
                }

                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        public void SetApiBase(string value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientCreateOptions));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __valueSpan;
                var __valueOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);

                fixed (NativeMethods.ClientCreateOptions* self = &this.self)
                {
                    NativeMethods.ClientCreateOptions.SetApiBase(self, __valueSpan);
                }

                NativeMethods.__FreeLocalString(&__valueSpan, __valueOwned);
            }
        }

        public AudioSystem ExperimentalAudioSystem()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientCreateOptions));
            }

            unsafe
            {
                AudioSystem __returnValue;

                fixed (NativeMethods.ClientCreateOptions* self = &this.self)
                {
                    __returnValue = NativeMethods.ClientCreateOptions.ExperimentalAudioSystem(self);
                }

                return __returnValue;
            }
        }

        public void SetExperimentalAudioSystem(AudioSystem value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientCreateOptions));
            }

            unsafe
            {
                fixed (NativeMethods.ClientCreateOptions* self = &this.self)
                {
                    NativeMethods.ClientCreateOptions.SetExperimentalAudioSystem(self, value);
                }
            }
        }

        public bool ExperimentalAndroidPreventCommsForBluetooth()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientCreateOptions));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.ClientCreateOptions* self = &this.self)
                {
                    __returnValue =
                        NativeMethods.ClientCreateOptions.ExperimentalAndroidPreventCommsForBluetooth(
                            self);
                }

                return __returnValue;
            }
        }

        public void SetExperimentalAndroidPreventCommsForBluetooth(bool value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientCreateOptions));
            }

            unsafe
            {
                fixed (NativeMethods.ClientCreateOptions* self = &this.self)
                {
                    NativeMethods.ClientCreateOptions.SetExperimentalAndroidPreventCommsForBluetooth(
                        self, value);
                }
            }
        }

        public ulong? CpuAffinityMask()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientCreateOptions));
            }

            unsafe
            {
                bool __returnIsNonNull;
                ulong __returnValue;

                fixed (NativeMethods.ClientCreateOptions* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.ClientCreateOptions.CpuAffinityMask(self, &__returnValue);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                return __returnValue;
            }
        }

        public void SetCpuAffinityMask(ulong? value)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(ClientCreateOptions));
            }

            unsafe
            {
                var __valueLocal = value ?? default;

                fixed (NativeMethods.ClientCreateOptions* self = &this.self)
                {
                    NativeMethods.ClientCreateOptions.SetCpuAffinityMask(
                        self, (value != null ? &__valueLocal : null));
                }
            }
        }
    }

    /// <summary>
    ///  The Client class is the main entry point for the Discord SDK. All functionality is exposed
    ///  through this class.
    /// </summary>
    /// <remarks>
    ///  See @ref getting_started "Getting Started" for more information on how to use the Client class.
    ///
    /// </remarks>
    public class Client : IDisposable
    {
        internal NativeMethods.Client self;
        private int disposed_;

        internal Client(NativeMethods.Client self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~Client() { Dispose(); }

        /// <summary>
        ///  Represents an error state for the socket connection that the Discord SDK maintains with the
        ///  Discord backend.
        /// </summary>
        /// <remarks>
        ///  Generic network failures will use the ConnectionFailed and ConnectionCanceled
        ///  enum values. Other errors such as if the user's auth token is invalid or out of
        ///  date will be UnexpectedClose and you should look at the other Error fields for the specific
        ///  details.
        ///
        /// </remarks>
        public enum Error
        {
            None = 0,
            ConnectionFailed = 1,
            UnexpectedClose = 2,
            ConnectionCanceled = 3,
        }

        /// <summary>
        ///  This enum refers to the status of the internal websocket the SDK uses to communicate with
        ///  Discord There are ~2 phases for "launching" the client:
        ///  1. The socket has to connect to Discord and exchange an auth token. This is indicated by
        ///  the `Connecting` and `Connected` values.
        ///  2. The socket has to receive an initial payload of data that describes the current user,
        ///  what lobbies they are in, who their friends are, etc. This is the `Ready` status.
        ///  Many Client functions will not work until the status changes to `Ready`, such as
        ///  GetCurrentUser().
        /// </summary>
        /// <remarks>
        ///  Status::Ready is the one you want to wait for!
        ///
        ///  Additionally, sometimes the socket will be disconnected, such as through temporary network
        ///  blips. But it will try to automatically reconnect, as indicated by the `Reconnecting`
        ///  status.
        ///
        /// </remarks>
        public enum Status
        {
            Disconnected = 0,
            Connecting = 1,
            Connected = 2,
            Ready = 3,
            Reconnecting = 4,
            Disconnecting = 5,
            HttpWait = 6,
        }

        /// <summary>
        ///  Represents the type of thread to control thread priority on.
        /// </summary>
        public enum Thread
        {
            Client = 0,
            Voice = 1,
            Network = 2,
        }

        public delegate void UpdateLobbyMemberCallback(ClientResult result,
                                                       ulong userId,
                                                       ulong lobbyId);

        public delegate void LobbyActionCallback(ClientResult result, ulong lobbyId);

        public delegate void PerformOnThreadWithStringCallback(string text);

        public delegate void UpdateUserApplicationProfileCallback(ClientResult result);

        public delegate void EndCallCallback();

        public delegate void EndCallsCallback();

        public delegate void GetCurrentInputDeviceCallback(AudioDevice device);

        public delegate void GetCurrentOutputDeviceCallback(AudioDevice device);

        public delegate void GetInputDevicesCallback(AudioDevice[] devices);

        public delegate void GetOutputDevicesCallback(AudioDevice[] devices);

        public delegate void DeviceChangeCallback(AudioDevice[] inputDevices,
                                                  AudioDevice[] outputDevices);

        public delegate void SetInputDeviceCallback(ClientResult result);

        public delegate void NoAudioInputCallback(bool inputDetected);

        public delegate void SetOutputDeviceCallback(ClientResult result);

        public delegate void VoiceParticipantChangedCallback(ulong lobbyId, ulong memberId, bool added);

        public delegate void UserAudioReceivedCallback(ulong userId,
                                                       IntPtr data,
                                                       ulong samplesPerChannel,
                                                       int sampleRate,
                                                       ulong channels,
                                                       ref bool outShouldMute);

        public delegate void UserAudioCapturedCallback(IntPtr data,
                                                       ulong samplesPerChannel,
                                                       int sampleRate,
                                                       ulong channels);

        public delegate void AuthorizationCallback(ClientResult result,
                                                   string code,
                                                   string redirectUri);

        public delegate void ExchangeChildTokenCallback(ClientResult result,
                                                        string accessToken,
                                                        AuthorizationTokenType tokenType,
                                                        int expiresIn,
                                                        string scopes);

        public delegate void FetchCurrentUserCallback(ClientResult result,
                                                      ulong id,
                                                      string name);

        public delegate void TokenExchangeCallback(ClientResult result,
                                                   string accessToken,
                                                   string refreshToken,
                                                   AuthorizationTokenType tokenType,
                                                   int expiresIn,
                                                   string scopes);

        public delegate void AuthorizeRequestCallback();

        public delegate void RevokeTokenCallback(ClientResult result);

        public delegate void AuthorizeDeviceScreenClosedCallback();

        public delegate void TokenExpirationCallback();

        public delegate void UnmergeIntoProvisionalAccountCallback(ClientResult result);

        public delegate void UpdateProvisionalAccountDisplayNameCallback(
            ClientResult result);

        public delegate void UpdateTokenCallback(ClientResult result);

        public delegate void DeleteUserMessageCallback(ClientResult result);

        public delegate void EditUserMessageCallback(ClientResult result);

        public delegate void GetLobbyMessagesCallback(ClientResult result,
                                                      MessageHandle[] messages);

        public delegate void UserMessageSummariesCallback(ClientResult result,
                                                          UserMessageSummary[] summaries);

        public delegate void UserMessagesWithLimitCallback(ClientResult result,
                                                           MessageHandle[] messages);

        public delegate void ProvisionalUserMergeRequiredCallback();

        public delegate void OpenMessageInDiscordCallback(ClientResult result);

        public delegate void SendUserMessageCallback(ClientResult result, ulong messageId);

        public delegate void MessageCreatedCallback(ulong messageId);

        public delegate void MessageDeletedCallback(ulong messageId, ulong channelId);

        public delegate void MessageUpdatedCallback(ulong messageId);

        public delegate void LogCallback(string message, LoggingSeverity severity);

        public delegate void OpenConnectedGamesSettingsInDiscordCallback(
            ClientResult result);

        public delegate void OnStatusChanged(Status status,
                                             Error error,
                                             int errorDetail);

        public delegate void CreateOrJoinLobbyCallback(ClientResult result, ulong lobbyId);

        public delegate void GetGuildChannelsCallback(ClientResult result,
                                                      GuildChannel[] guildChannels);

        public delegate void GetUserGuildsCallback(ClientResult result,
                                                   GuildMinimal[] guilds);

        public delegate void JoinLinkedLobbyGuildCallback(ClientResult result,
                                                          string inviteUrl);

        public delegate void LeaveLobbyCallback(ClientResult result);

        public delegate void LinkOrUnlinkChannelCallback(ClientResult result);

        public delegate void LobbyCreatedCallback(ulong lobbyId);

        public delegate void LobbyDeletedCallback(ulong lobbyId);

        public delegate void LobbyMemberAddedCallback(ulong lobbyId, ulong memberId);

        public delegate void LobbyMemberRemovedCallback(ulong lobbyId, ulong memberId);

        public delegate void LobbyMemberUpdatedCallback(ulong lobbyId, ulong memberId);

        public delegate void LobbyUpdatedCallback(ulong lobbyId);

        public delegate void IsDiscordAppInstalledCallback(bool installed);

        public delegate void AcceptActivityInviteCallback(ClientResult result,
                                                          string joinSecret);

        public delegate void SendActivityInviteCallback(ClientResult result);

        public delegate void ActivityInviteCallback(ActivityInvite invite);

        public delegate void ActivityJoinCallback(string joinSecret);

        public delegate void ActivityJoinWithApplicationCallback(ulong applicationId,
                                                                 string joinSecret);

        public delegate void UpdateStatusCallback(ClientResult result);

        public delegate void UpdateRichPresenceCallback(ClientResult result);

        public delegate void UpdateRelationshipCallback(ClientResult result);

        public delegate void SendFriendRequestCallback(ClientResult result);

        public delegate void RelationshipCreatedCallback(ulong userId,
                                                         bool isDiscordRelationshipUpdate);

        public delegate void RelationshipDeletedCallback(ulong userId,
                                                         bool isDiscordRelationshipUpdate);

        public delegate void GetDiscordClientConnectedUserCallback(ClientResult result,
                                                                   UserHandle? user);

        public delegate void RelationshipGroupsUpdatedCallback(ulong userId);

        public delegate void UserUpdatedCallback(ulong userId);

        /// <summary>
        ///  Creates a new instance of the Client.
        /// </summary>
        public Client()
        {
            NativeMethods.__Init();

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.Init(self);
                }
            }

            NativeMethods.__OnPostConstruct(this);
        }

        /// <summary>
        ///  Creates a new instance of the Client but allows customizing the Discord URL to use.
        /// </summary>
        public Client(string apiBase, string webBase)
        {
            NativeMethods.__Init();

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __apiBaseSpan;
                var __apiBaseOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__apiBaseSpan, apiBase);
                NativeMethods.Discord_String __webBaseSpan;
                var __webBaseOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__webBaseSpan, webBase);

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.InitWithBases(self, __apiBaseSpan, __webBaseSpan);
                }

                NativeMethods.__FreeLocalString(&__webBaseSpan, __webBaseOwned);
                NativeMethods.__FreeLocalString(&__apiBaseSpan, __apiBaseOwned);
            }

            NativeMethods.__OnPostConstruct(this);
        }

        /// <summary>
        ///  Creates a new instance of the Client with custom options.
        /// </summary>
        public Client(ClientCreateOptions options)
        {
            NativeMethods.__Init();

            unsafe
            {
                fixed (NativeMethods.ClientCreateOptions* __optionsFixed = &options.self)
                {
                    fixed (NativeMethods.Client* self = &this.self)
                    {
                        NativeMethods.Client.InitWithOptions(self, __optionsFixed);
                    }
                }
            }

            NativeMethods.__OnPostConstruct(this);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.Drop(self);
                }
            }
        }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public void AddOrUpdateLobbyMember(string token,
                                       ulong userId,
                                       ulong lobbyId,
                                       Dictionary<string, string> metadata,
                                       Discord.Sdk.LobbyMemberFlags flags,
                                       Discord.Sdk.Client.UpdateLobbyMemberCallback callback) {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe {
            var __scratchAligned = stackalloc ulong[128];
            var __scratch = (byte*)__scratchAligned;
            var __scratchUsed = 0;
            NativeMethods.Discord_String __tokenSpan;
            var __tokenOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__tokenSpan, token);
            Discord.Sdk.NativeMethods.Discord_Properties __metadataNative;
            __metadataNative.size = (IntPtr)metadata.Count;
            NativeMethods.Discord_String* __metadataKeys;
            NativeMethods.Discord_String* __metadataValues;
            bool* __metadataKeyOwnership;
            bool* __metadataValueOwnership;
            var __metadataKeysOwned = NativeMethods.__AllocLocalStringArray(
              __scratch, &__scratchUsed, 1024, &__metadataKeys, metadata.Count);
            var __metadataValuesOwned = NativeMethods.__AllocLocalStringArray(
              __scratch, &__scratchUsed, 1024, &__metadataValues, metadata.Count);
            var __metadataKeyOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
              __scratch, &__scratchUsed, 1024, &__metadataKeyOwnership, metadata.Count);
            var __metadataValueOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
              __scratch, &__scratchUsed, 1024, &__metadataValueOwnership, metadata.Count);
            {
                int __i = 0;
                foreach (var (__metadataKey, __metadataValue) in metadata) {
                    NativeMethods.Discord_String __metadataKeySpan;
                    NativeMethods.Discord_String __metadataValueSpan;
                    __metadataKeyOwnership[__i] = NativeMethods.__InitStringLocal(
                      __scratch, &__scratchUsed, 1024, &__metadataKeySpan, __metadataKey);
                    __metadataValueOwnership[__i] = NativeMethods.__InitStringLocal(
                      __scratch, &__scratchUsed, 1024, &__metadataValueSpan, __metadataValue);
                    __metadataKeys[__i] = __metadataKeySpan;
                    __metadataValues[__i] = __metadataValueSpan;
                    __i++;
                }
            }
            __metadataNative.keys = __metadataKeys;
            __metadataNative.values = __metadataValues;
            Discord.Sdk.NativeMethods.Client.UpdateLobbyMemberCallback __callbackDelegate =
              Discord.Sdk.NativeMethods.Client.UpdateLobbyMemberCallback_Handler;
            fixed(NativeMethods.Client* self = &this.self) {
                NativeMethods.Client.AddOrUpdateLobbyMember(
                  self,
                  __tokenSpan,
                  userId,
                  lobbyId,
                  __metadataNative,
                  flags,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
            for (int __i = 0; __i < (int)__metadataNative.size; __i++) {
                NativeMethods.__FreeLocalString(&__metadataKeys[__i], __metadataKeyOwnership[__i]);
                NativeMethods.__FreeLocalString(&__metadataValues[__i],
                                                __metadataValueOwnership[__i]);
            }
            NativeMethods.__FreeLocal(__metadataKeys, __metadataKeysOwned);
            NativeMethods.__FreeLocal(__metadataValues, __metadataValuesOwned);
            NativeMethods.__FreeLocalString(&__tokenSpan, __tokenOwned);
        }
    }
#endif
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public void CreateNewLobby(string token, Discord.Sdk.Client.LobbyActionCallback callback) {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe {
            var __scratchAligned = stackalloc ulong[128];
            var __scratch = (byte*)__scratchAligned;
            var __scratchUsed = 0;
            NativeMethods.Discord_String __tokenSpan;
            var __tokenOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__tokenSpan, token);
            Discord.Sdk.NativeMethods.Client.LobbyActionCallback __callbackDelegate =
              Discord.Sdk.NativeMethods.Client.LobbyActionCallback_Handler;
            fixed(NativeMethods.Client* self = &this.self) {
                NativeMethods.Client.CreateNewLobby(
                  self,
                  __tokenSpan,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
            NativeMethods.__FreeLocalString(&__tokenSpan, __tokenOwned);
        }
    }
#endif
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public void DeleteLobby(string token,
                            ulong lobbyId,
                            Discord.Sdk.Client.LobbyActionCallback callback) {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe {
            var __scratchAligned = stackalloc ulong[128];
            var __scratch = (byte*)__scratchAligned;
            var __scratchUsed = 0;
            NativeMethods.Discord_String __tokenSpan;
            var __tokenOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__tokenSpan, token);
            Discord.Sdk.NativeMethods.Client.LobbyActionCallback __callbackDelegate =
              Discord.Sdk.NativeMethods.Client.LobbyActionCallback_Handler;
            fixed(NativeMethods.Client* self = &this.self) {
                NativeMethods.Client.DeleteLobby(
                  self,
                  __tokenSpan,
                  lobbyId,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
            NativeMethods.__FreeLocalString(&__tokenSpan, __tokenOwned);
        }
    }
#endif
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public void EditLobby(string token,
                          ulong lobbyId,
                          Dictionary<string, string> lobbyMetadata,
                          Discord.Sdk.Client.LobbyActionCallback callback) {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe {
            var __scratchAligned = stackalloc ulong[128];
            var __scratch = (byte*)__scratchAligned;
            var __scratchUsed = 0;
            NativeMethods.Discord_String __tokenSpan;
            var __tokenOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__tokenSpan, token);
            Discord.Sdk.NativeMethods.Discord_Properties __lobbyMetadataNative;
            __lobbyMetadataNative.size = (IntPtr)lobbyMetadata.Count;
            NativeMethods.Discord_String* __lobbyMetadataKeys;
            NativeMethods.Discord_String* __lobbyMetadataValues;
            bool* __lobbyMetadataKeyOwnership;
            bool* __lobbyMetadataValueOwnership;
            var __lobbyMetadataKeysOwned = NativeMethods.__AllocLocalStringArray(
              __scratch, &__scratchUsed, 1024, &__lobbyMetadataKeys, lobbyMetadata.Count);
            var __lobbyMetadataValuesOwned = NativeMethods.__AllocLocalStringArray(
              __scratch, &__scratchUsed, 1024, &__lobbyMetadataValues, lobbyMetadata.Count);
            var __lobbyMetadataKeyOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
              __scratch, &__scratchUsed, 1024, &__lobbyMetadataKeyOwnership, lobbyMetadata.Count);
            var __lobbyMetadataValueOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
              __scratch, &__scratchUsed, 1024, &__lobbyMetadataValueOwnership, lobbyMetadata.Count);
            {
                int __i = 0;
                foreach (var (__lobbyMetadataKey, __lobbyMetadataValue) in lobbyMetadata) {
                    NativeMethods.Discord_String __lobbyMetadataKeySpan;
                    NativeMethods.Discord_String __lobbyMetadataValueSpan;
                    __lobbyMetadataKeyOwnership[__i] = NativeMethods.__InitStringLocal(
                      __scratch, &__scratchUsed, 1024, &__lobbyMetadataKeySpan, __lobbyMetadataKey);
                    __lobbyMetadataValueOwnership[__i] =
                      NativeMethods.__InitStringLocal(__scratch,
                                                      &__scratchUsed,
                                                      1024,
                                                      &__lobbyMetadataValueSpan,
                                                      __lobbyMetadataValue);
                    __lobbyMetadataKeys[__i] = __lobbyMetadataKeySpan;
                    __lobbyMetadataValues[__i] = __lobbyMetadataValueSpan;
                    __i++;
                }
            }
            __lobbyMetadataNative.keys = __lobbyMetadataKeys;
            __lobbyMetadataNative.values = __lobbyMetadataValues;
            Discord.Sdk.NativeMethods.Client.LobbyActionCallback __callbackDelegate =
              Discord.Sdk.NativeMethods.Client.LobbyActionCallback_Handler;
            fixed(NativeMethods.Client* self = &this.self) {
                NativeMethods.Client.EditLobby(
                  self,
                  __tokenSpan,
                  lobbyId,
                  __lobbyMetadataNative,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
            for (int __i = 0; __i < (int)__lobbyMetadataNative.size; __i++) {
                NativeMethods.__FreeLocalString(&__lobbyMetadataKeys[__i],
                                                __lobbyMetadataKeyOwnership[__i]);
                NativeMethods.__FreeLocalString(&__lobbyMetadataValues[__i],
                                                __lobbyMetadataValueOwnership[__i]);
            }
            NativeMethods.__FreeLocal(__lobbyMetadataKeys, __lobbyMetadataKeysOwned);
            NativeMethods.__FreeLocal(__lobbyMetadataValues, __lobbyMetadataValuesOwned);
            NativeMethods.__FreeLocalString(&__tokenSpan, __tokenOwned);
        }
    }
#endif
        /// <summary>
        ///  Converts the Error enum to a string.
        /// </summary>
        public static string ErrorToString(Error type)
        {
            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();
                NativeMethods.Client.ErrorToString(type, &__returnValue);
                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public void ForceCrash_ForTest() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe {
            fixed(NativeMethods.Client* self = &this.self) {
                NativeMethods.Client.ForceCrash_ForTest(self);
            }
        }
    }
#endif
        /// <summary>
        ///  This function is used to get the application ID for the client. This is used to
        ///  identify the application to the Discord client. This is used for things like
        ///  authentication, rich presence, and activity invites when *not* connected with
        ///  Client::Connect. When calling Client::Connect, the application ID is set automatically
        /// </summary>
        public ulong GetApplicationId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnValue = NativeMethods.Client.GetApplicationId(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        /// (deprecated)
        /// </summary>
        /// <remarks>
        ///  \deprecated Please use GetCurrentUserV2 instead. This will be removed in a future version.
        ///
        /// </remarks>
        [Obsolete("Please use GetCurrentUserV2 instead. This will be removed in a future version.")]
        public UserHandle GetCurrentUser()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __returnValueNative = new NativeMethods.UserHandle();
                UserHandle? __returnValue = null;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetCurrentUser(self, &__returnValueNative);
                }

                __returnValue = new UserHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the ID of the system default audio device if the user has not explicitly chosen
        ///  one.
        /// </summary>
        public static string GetDefaultAudioDeviceId()
        {
            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();
                NativeMethods.Client.GetDefaultAudioDeviceId(&__returnValue);
                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns the default set of OAuth2 scopes that should be used with the Discord SDK
        ///  when making use of the full SDK capabilities, including communications-related features
        ///  (e.g. user DMs, lobbies, voice chat). If your application does not make use of these
        ///  features, you should use Client::GetDefaultPresenceScopes instead.
        /// </summary>
        /// <remarks>
        ///  Communications-related features are currently in limited access and are not available to
        ///  all applications, however, they can be demoed in limited capacity by all applications. If
        ///  you are interested in using these features in your game, please reach out to the Discord
        ///  team.
        ///
        ///  It's ok to further customize your requested oauth2 scopes to add additional scopes if you
        ///  have legitimate usages for them. However, we strongly recommend always using
        ///  Client::GetDefaultCommunicationScopes or Client::GetDefaultPresenceScopes as a baseline to
        ///  enable a better authorization experience for users!
        ///
        /// </remarks>
        public static string GetDefaultCommunicationScopes()
        {
            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();
                NativeMethods.Client.GetDefaultCommunicationScopes(&__returnValue);
                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns the default set of OAuth2 scopes that should be used with the Discord SDK
        ///  when leveraging baseline presence-related features (e.g. friends list, rich presence,
        ///  provisional accounts, activity invites). If your application is using
        ///  communications-related features, which are currently available in limited access, you
        ///  should use Client::GetDefaultCommunicationScopes instead.
        /// </summary>
        /// <remarks>
        ///  It's ok to further customize your requested oauth2 scopes to add additional scopes if you
        ///  have legitimate usages for them. However, we strongly recommend always using
        ///  Client::GetDefaultCommunicationScopes or Client::GetDefaultPresenceScopes as a baseline to
        ///  enable a better authorization experience for users!
        ///
        /// </remarks>
        public static string GetDefaultPresenceScopes()
        {
            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();
                NativeMethods.Client.GetDefaultPresenceScopes(&__returnValue);
                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public Discord.Sdk.GuildHandle? GetGuildHandle(ulong id) {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.GuildHandle();
            GuildHandle? __returnValue = null;
            fixed(NativeMethods.Client* self = &this.self) {
                __returnIsNonNull =
                  NativeMethods.Client.GetGuildHandle(self, id, &__returnValueNative);
            }
            if (!__returnIsNonNull) {
                return null;
            }
            __returnValue = new GuildHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
#endif
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public ulong[] GetGuildIds() {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe {
            var __returnValue = new NativeMethods.Discord_UInt64Span();
            fixed(NativeMethods.Client* self = &this.self) {
                NativeMethods.Client.GetGuildIds(self, &__returnValue);
            }
            var __returnValueSurface =
              new Span<ulong>(__returnValue.ptr, (int)__returnValue.size).ToArray();
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
#endif
        /// <summary>
        ///  Returns the git commit hash this version was built from.
        /// </summary>
        public static string GetVersionHash()
        {
            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();
                NativeMethods.Client.GetVersionHash(&__returnValue);
                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns the major version of the Discord Social SDK.
        /// </summary>
        public static int GetVersionMajor()
        {
            unsafe
            {
                int __returnValue;
                __returnValue = NativeMethods.Client.GetVersionMajor();
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the minor version of the Discord Social SDK.
        /// </summary>
        public static int GetVersionMinor()
        {
            unsafe
            {
                int __returnValue;
                __returnValue = NativeMethods.Client.GetVersionMinor();
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the patch version of the Discord Social SDK.
        /// </summary>
        public static int GetVersionPatch()
        {
            unsafe
            {
                int __returnValue;
                __returnValue = NativeMethods.Client.GetVersionPatch();
                return __returnValue;
            }
        }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public void PerformOnThreadWithString_ForTest(
      string text,
      Discord.Sdk.Client.PerformOnThreadWithStringCallback callback) {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe {
            var __scratchAligned = stackalloc ulong[128];
            var __scratch = (byte*)__scratchAligned;
            var __scratchUsed = 0;
            NativeMethods.Discord_String __textSpan;
            var __textOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__textSpan, text);
            Discord.Sdk.NativeMethods.Client.PerformOnThreadWithStringCallback __callbackDelegate =
              Discord.Sdk.NativeMethods.Client.PerformOnThreadWithStringCallback_Handler;
            fixed(NativeMethods.Client* self = &this.self) {
                NativeMethods.Client.PerformOnThreadWithString_ForTest(
                  self,
                  __textSpan,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
            NativeMethods.__FreeLocalString(&__textSpan, __textOwned);
        }
    }
#endif
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public void RemoveLobbyMember(string token,
                                  ulong userId,
                                  ulong lobbyId,
                                  Discord.Sdk.Client.UpdateLobbyMemberCallback callback) {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe {
            var __scratchAligned = stackalloc ulong[128];
            var __scratch = (byte*)__scratchAligned;
            var __scratchUsed = 0;
            NativeMethods.Discord_String __tokenSpan;
            var __tokenOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__tokenSpan, token);
            Discord.Sdk.NativeMethods.Client.UpdateLobbyMemberCallback __callbackDelegate =
              Discord.Sdk.NativeMethods.Client.UpdateLobbyMemberCallback_Handler;
            fixed(NativeMethods.Client* self = &this.self) {
                NativeMethods.Client.RemoveLobbyMember(
                  self,
                  __tokenSpan,
                  userId,
                  lobbyId,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
            NativeMethods.__FreeLocalString(&__tokenSpan, __tokenOwned);
        }
    }
#endif
        /// <summary>
        ///  This function is used to override the default HTTP timeout for the websocket client.
        /// </summary>
        public void SetHttpRequestTimeout(int httpTimeoutInMilliseconds)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetHttpRequestTimeout(self, httpTimeoutInMilliseconds);
                }
            }
        }

        /// <summary>
        ///  Converts the Status enum to a string.
        /// </summary>
        public static string StatusToString(Status type)
        {
            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();
                NativeMethods.Client.StatusToString(type, &__returnValue);
                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Converts the Thread enum to a string.
        /// </summary>
        public static string ThreadToString(Thread type)
        {
            unsafe
            {
                var __returnValue = new NativeMethods.Discord_String();
                NativeMethods.Client.ThreadToString(type, &__returnValue);
                string __returnValueSurface =
                    Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
                NativeMethods.Discord_Free((void*)__returnValue.ptr);
                return __returnValueSurface;
            }
        }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public void UpdateUserApplicationProfile(
      string token,
      ulong applicationId,
      ulong userId,
      string providerIssuedUserId,
      string username,
      Dictionary<string, string> metadata,
      Discord.Sdk.Client.UpdateUserApplicationProfileCallback cb) {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe {
            var __scratchAligned = stackalloc ulong[128];
            var __scratch = (byte*)__scratchAligned;
            var __scratchUsed = 0;
            NativeMethods.Discord_String __tokenSpan;
            var __tokenOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__tokenSpan, token);
            NativeMethods.Discord_String __providerIssuedUserIdSpan;
            var __providerIssuedUserIdOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__providerIssuedUserIdSpan, providerIssuedUserId);
            NativeMethods.Discord_String __usernameSpan;
            var __usernameOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__usernameSpan, username);
            Discord.Sdk.NativeMethods.Discord_Properties __metadataNative;
            __metadataNative.size = (IntPtr)metadata.Count;
            NativeMethods.Discord_String* __metadataKeys;
            NativeMethods.Discord_String* __metadataValues;
            bool* __metadataKeyOwnership;
            bool* __metadataValueOwnership;
            var __metadataKeysOwned = NativeMethods.__AllocLocalStringArray(
              __scratch, &__scratchUsed, 1024, &__metadataKeys, metadata.Count);
            var __metadataValuesOwned = NativeMethods.__AllocLocalStringArray(
              __scratch, &__scratchUsed, 1024, &__metadataValues, metadata.Count);
            var __metadataKeyOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
              __scratch, &__scratchUsed, 1024, &__metadataKeyOwnership, metadata.Count);
            var __metadataValueOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
              __scratch, &__scratchUsed, 1024, &__metadataValueOwnership, metadata.Count);
            {
                int __i = 0;
                foreach (var (__metadataKey, __metadataValue) in metadata) {
                    NativeMethods.Discord_String __metadataKeySpan;
                    NativeMethods.Discord_String __metadataValueSpan;
                    __metadataKeyOwnership[__i] = NativeMethods.__InitStringLocal(
                      __scratch, &__scratchUsed, 1024, &__metadataKeySpan, __metadataKey);
                    __metadataValueOwnership[__i] = NativeMethods.__InitStringLocal(
                      __scratch, &__scratchUsed, 1024, &__metadataValueSpan, __metadataValue);
                    __metadataKeys[__i] = __metadataKeySpan;
                    __metadataValues[__i] = __metadataValueSpan;
                    __i++;
                }
            }
            __metadataNative.keys = __metadataKeys;
            __metadataNative.values = __metadataValues;
            Discord.Sdk.NativeMethods.Client.UpdateUserApplicationProfileCallback __cbDelegate =
              Discord.Sdk.NativeMethods.Client.UpdateUserApplicationProfileCallback_Handler;
            fixed(NativeMethods.Client* self = &this.self) {
                NativeMethods.Client.UpdateUserApplicationProfile(
                  self,
                  __tokenSpan,
                  applicationId,
                  userId,
                  __providerIssuedUserIdSpan,
                  __usernameSpan,
                  __metadataNative,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
            for (int __i = 0; __i < (int)__metadataNative.size; __i++) {
                NativeMethods.__FreeLocalString(&__metadataKeys[__i], __metadataKeyOwnership[__i]);
                NativeMethods.__FreeLocalString(&__metadataValues[__i],
                                                __metadataValueOwnership[__i]);
            }
            NativeMethods.__FreeLocal(__metadataKeys, __metadataKeysOwned);
            NativeMethods.__FreeLocal(__metadataValues, __metadataValuesOwned);
            NativeMethods.__FreeLocalString(&__usernameSpan, __usernameOwned);
            NativeMethods.__FreeLocalString(&__providerIssuedUserIdSpan,
                                            __providerIssuedUserIdOwned);
            NativeMethods.__FreeLocalString(&__tokenSpan, __tokenOwned);
        }
    }
#endif
        /// <summary>
        ///  Ends any active call, if any. Any references you have to Call objects are invalid after
        ///  they are ended, and can be immediately freed.
        /// </summary>
        public void EndCall(ulong channelId, EndCallCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.EndCallCallback __callbackDelegate =
                    NativeMethods.Client.EndCallCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.EndCall(self,
                        channelId,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }

        /// <summary>
        ///  Ends any active call, if any. Any references you have to Call objects are invalid after
        ///  they are ended, and can be immediately freed.
        /// </summary>
        public void EndCalls(EndCallsCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.EndCallsCallback __callbackDelegate =
                    NativeMethods.Client.EndCallsCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.EndCalls(self,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }

        /// <summary>
        ///  Returns a reference to the currently active call, if any.
        /// </summary>
        public Call? GetCall(ulong channelId)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.Call();
                Call? __returnValue = null;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.Client.GetCall(self, channelId, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new Call(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns a reference to all currently active calls, if any.
        /// </summary>
        public Call?[] GetCalls()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_CallSpan();

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetCalls(self, &__returnValue);
                }

                var __returnValueSurface = new Call?[(int)__returnValue.size];

                for (int __i = 0; __i < (int)__returnValue.size; __i++)
                {
                    __returnValueSurface[__i] = new Call(__returnValue.ptr[__i], 0);
                }

                NativeMethods.Discord_Free(__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Asynchronously fetches the current audio input device in use by the client.
        /// </summary>
        public void GetCurrentInputDevice(GetCurrentInputDeviceCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.GetCurrentInputDeviceCallback __cbDelegate =
                    NativeMethods.Client.GetCurrentInputDeviceCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetCurrentInputDevice(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Asynchronously fetches the current audio output device in use by the client.
        /// </summary>
        public void GetCurrentOutputDevice(GetCurrentOutputDeviceCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.GetCurrentOutputDeviceCallback __cbDelegate =
                    NativeMethods.Client.GetCurrentOutputDeviceCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetCurrentOutputDevice(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Asynchronously fetches the list of audio input devices available to the user.
        /// </summary>
        public void GetInputDevices(GetInputDevicesCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.GetInputDevicesCallback __cbDelegate =
                    NativeMethods.Client.GetInputDevicesCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetInputDevices(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Returns the input volume for the current user's microphone.
        /// </summary>
        /// <remarks>
        ///  Input volume is specified as a percentage in the range [0, 100] which represents the
        ///  perceptual loudness.
        ///
        /// </remarks>
        public float GetInputVolume()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                float __returnValue;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnValue = NativeMethods.Client.GetInputVolume(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Asynchronously fetches the list of audio output devices available to the user.
        /// </summary>
        public void GetOutputDevices(GetOutputDevicesCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.GetOutputDevicesCallback __cbDelegate =
                    NativeMethods.Client.GetOutputDevicesCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetOutputDevices(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Returns the output volume for the current user.
        /// </summary>
        /// <remarks>
        ///  Output volume specified as a percentage in the range [0, 200] which represents the
        ///  perceptual loudness.
        ///
        /// </remarks>
        public float GetOutputVolume()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                float __returnValue;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnValue = NativeMethods.Client.GetOutputVolume(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns whether the current user is deafened in all calls.
        /// </summary>
        public bool GetSelfDeafAll()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnValue = NativeMethods.Client.GetSelfDeafAll(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns whether the current user's microphone is muted in all calls.
        /// </summary>
        public bool GetSelfMuteAll()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnValue = NativeMethods.Client.GetSelfMuteAll(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Enables or disables AEC diagnostic recording.
        /// </summary>
        /// <remarks>
        ///  Used to diagnose issues with acoustic echo cancellation. The input and output waveform data
        ///  will be written to the log directory.
        ///
        /// </remarks>
        public void SetAecDump(bool on)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetAecDump(self, on);
                }
            }
        }

        /// <summary>
        ///  When enabled, automatically adjusts the microphone volume to keep it clear and consistent.
        /// </summary>
        /// <remarks>
        ///  Defaults to on.
        ///
        ///  Generally this shouldn't need to be used unless you are building a voice settings UI for
        ///  the user to control, similar to Discord's voice settings.
        ///
        /// </remarks>
        public void SetAutomaticGainControl(bool on)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetAutomaticGainControl(self, on);
                }
            }
        }

        /// <summary>
        ///  Sets a callback function to be invoked when Discord detects a change in the available audio
        ///  devices.
        /// </summary>
        public void SetDeviceChangeCallback(DeviceChangeCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.DeviceChangeCallback __callbackDelegate =
                    NativeMethods.Client.DeviceChangeCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetDeviceChangeCallback(
                        self,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }

        /// <summary>
        ///  Enables or disables the basic echo cancellation provided by the WebRTC library.
        /// </summary>
        /// <remarks>
        ///  Defaults to on.
        ///
        ///  Generally this shouldn't need to be used unless you are building a voice settings UI for
        ///  the user to control, similar to Discord's voice settings.
        ///
        /// </remarks>
        public void SetEchoCancellation(bool on)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetEchoCancellation(self, on);
                }
            }
        }

        /// <summary>
        ///  On mobile devices, set whether the audio environment is managed by the engine or the SDK.
        ///  On Android, this entails AudioManager state and on iOS, this entails AVAudioSession
        ///  activation.
        /// </summary>
        /// <remarks>
        ///  This method must be called before connecting to any Calls if the
        ///  application manages audio on its own, otherwise audio management
        ///  will be ended by the voice engine when the last Call is ended.
        ///
        ///  The Unity plugin automatically calls this method if the native Unity
        ///  audio engine is enabled in the project settings.
        ///
        /// </remarks>
        public void SetEngineManagedAudioSession(bool isEngineManaged)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetEngineManagedAudioSession(self, isEngineManaged);
                }
            }
        }

        /// <summary>
        ///  Asynchronously changes the audio input device in use by the client to the specified device.
        ///  You can find the list of device IDs that can be passed in with the Client::GetInputDevices
        ///  function.
        /// </summary>
        public void SetInputDevice(string deviceId, SetInputDeviceCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __deviceIdSpan;
                var __deviceIdOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__deviceIdSpan, deviceId);
                Discord.Sdk.NativeMethods.Client.SetInputDeviceCallback __cbDelegate =
                    NativeMethods.Client.SetInputDeviceCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetInputDevice(self,
                        __deviceIdSpan,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }

                NativeMethods.__FreeLocalString(&__deviceIdSpan, __deviceIdOwned);
            }
        }

        /// <summary>
        ///  Sets the microphone volume for the current user.
        /// </summary>
        /// <remarks>
        ///  Input volume is specified as a percentage in the range [0, 100] which represents the
        ///  perceptual loudness.
        ///
        /// </remarks>
        public void SetInputVolume(float inputVolume)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetInputVolume(self, inputVolume);
                }
            }
        }

        /// <summary>
        ///  Callback function invoked when the above threshold is set and there is a change in whether
        ///  audio is being detected.
        /// </summary>
        public void SetNoAudioInputCallback(NoAudioInputCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.NoAudioInputCallback __callbackDelegate =
                    NativeMethods.Client.NoAudioInputCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetNoAudioInputCallback(
                        self,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }

        /// <summary>
        ///  Threshold that can be set to indicate when no audio is being received by the user's mic.
        /// </summary>
        /// <remarks>
        ///  An example of when this may be useful: When push to talk is being used and the user pushes
        ///  their talk key, but something is configured wrong and no audio is being received, this
        ///  threshold and callback can be used to detect that situation and notify the user. The
        ///  threshold is specified in DBFS, or decibels relative to full scale, and the range is
        ///  [-100.0, 100.0] It defaults to -100.0, so is disabled.
        ///
        /// </remarks>
        public void SetNoAudioInputThreshold(float dBFSThreshold)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetNoAudioInputThreshold(self, dBFSThreshold);
                }
            }
        }

        /// <summary>
        ///  Enables or disables Krisp noise cancellation.
        /// </summary>
        /// <remarks>
        ///  Defaults to off. When enabled, noise suppression is automatically disabled.
        ///
        /// </remarks>
        public void SetNoiseCancellation(bool on)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetNoiseCancellation(self, on);
                }
            }
        }

        /// <summary>
        ///  Enables basic background noise suppression.
        /// </summary>
        /// <remarks>
        ///  Defaults to on.
        ///
        ///  Generally this shouldn't need to be used unless you are building a voice settings UI for
        ///  the user to control, similar to Discord's voice settings.
        ///
        /// </remarks>
        public void SetNoiseSuppression(bool on)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetNoiseSuppression(self, on);
                }
            }
        }

        /// <summary>
        ///  Enables or disables hardware encoding and decoding for audio, if it is available.
        /// </summary>
        /// <remarks>
        ///  Defaults to on.
        ///
        ///  This must be called immediately after constructing the Client. If called too late an error
        ///  will be logged and the setting will not take effect.
        ///
        /// </remarks>
        public void SetOpusHardwareCoding(bool encode, bool decode)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetOpusHardwareCoding(self, encode, decode);
                }
            }
        }

        /// <summary>
        ///  Asynchronously changes the audio output device in use by the client to the specified
        ///  device. You can find the list of device IDs that can be passed in with the
        ///  Client::GetOutputDevices function.
        /// </summary>
        public void SetOutputDevice(string deviceId, SetOutputDeviceCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __deviceIdSpan;
                var __deviceIdOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__deviceIdSpan, deviceId);
                Discord.Sdk.NativeMethods.Client.SetOutputDeviceCallback __cbDelegate =
                    NativeMethods.Client.SetOutputDeviceCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetOutputDevice(
                        self,
                        __deviceIdSpan,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }

                NativeMethods.__FreeLocalString(&__deviceIdSpan, __deviceIdOwned);
            }
        }

        /// <summary>
        ///  Sets the speaker volume for the current user.
        /// </summary>
        /// <remarks>
        ///  Output volume specified as a percentage in the range [0, 200] which represents the
        ///  perceptual loudness.
        ///
        /// </remarks>
        public void SetOutputVolume(float outputVolume)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetOutputVolume(self, outputVolume);
                }
            }
        }

        /// <summary>
        ///  Mutes all audio from the currently active call for the current user in all calls.
        ///  They will not be able to hear any other participants,
        ///  and no other participants will be able to hear the current user either.
        ///  Note: This overrides the per-call setting.
        /// </summary>
        public void SetSelfDeafAll(bool deaf)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetSelfDeafAll(self, deaf);
                }
            }
        }

        /// <summary>
        ///  Mutes the current user's microphone so that no other participant in their active calls can
        ///  hear them in all calls.
        ///  Note: This overrides the per-call setting.
        /// </summary>
        public void SetSelfMuteAll(bool mute)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetSelfMuteAll(self, mute);
                }
            }
        }

        /// <summary>
        /// (deprecated)  On mobile devices, enable speakerphone mode.
        /// </summary>
        /// <remarks>
        ///  \deprecated Calling Client::SetSpeakerMode is DEPRECATED.
        ///
        /// </remarks>
        [Obsolete("Calling Client::SetSpeakerMode is DEPRECATED.")]
        public bool SetSpeakerMode(bool speakerMode)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnValue = NativeMethods.Client.SetSpeakerMode(self, speakerMode);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Allows setting the priority of various SDK threads.
        /// </summary>
        /// <remarks>
        ///  The threads that can be controlled are:
        ///  - Client: This is the main thread for the SDK where most of the data processing happens
        ///  - Network: This is the thread that receives voice data from lobby calls
        ///  - Voice: This is the thread that the voice engine runs on and processes all audio data
        ///
        /// </remarks>
        public void SetThreadPriority(Thread thread, int priority)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetThreadPriority(self, thread, priority);
                }
            }
        }

        /// <summary>
        ///  Callback invoked whenever a user in a lobby joins or leaves a voice call.
        /// </summary>
        /// <remarks>
        ///  The main use case for this is to enable displaying which users are in voice in a lobby
        ///  even if the current user is not in voice yet, and thus does not have a Call object to bind
        ///  to.
        ///
        /// </remarks>
        public void SetVoiceParticipantChangedCallback(
            VoiceParticipantChangedCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.VoiceParticipantChangedCallback __cbDelegate =
                    NativeMethods.Client.VoiceParticipantChangedCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetVoiceParticipantChangedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  On iOS devices, show the system audio route picker.
        /// </summary>
        public bool ShowAudioRoutePicker()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnValue = NativeMethods.Client.ShowAudioRoutePicker(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Starts or joins a call in the lobby specified by channelId (For a lobby, simply
        ///  pass in the lobbyId).
        /// </summary>
        /// <remarks>
        ///  On iOS, your application is responsible for enabling the appropriate background audio mode
        ///  in your Info.plist. VoiceBuildPostProcessor in the sample demonstrates how to do this
        ///  automatically in your Unity build process.
        ///
        ///  On macOS, you should set the NSMicrophoneUsageDescription key in your Info.plist.
        ///
        ///  Returns null if the user is already in the given voice channel.
        ///
        /// </remarks>
        public Call? StartCall(ulong channelId)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.Call();
                Call? __returnValue = null;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.Client.StartCall(self, channelId, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new Call(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Starts or joins a call in the specified lobby.
        /// </summary>
        /// <remarks>
        ///  The audio received callback is invoked whenever incoming audio is received in a call. If
        ///  the developer sets outShouldMute to true during the callback, the audio data will be muted
        ///  after the callback is invoked, which is useful if the developer is utilizing the incoming
        ///  audio and playing it through their own audio engine or playback. The audio samples
        ///  in `data` can be modified in-place for simple DSP effects.
        ///
        ///  The audio captured callback is invoked whenever local audio is captured before it is
        ///  processed and transmitted which may be useful for voice moderation, etc.
        ///
        ///  On iOS, your application is responsible for enabling the appropriate background audio mode
        ///  in your Info.plist. VoiceBuildPostProcessor in the sample demonstrates how to do this
        ///  automatically in your Unity build process.
        ///
        ///  On macOS, you should set the NSMicrophoneUsageDescription key in your Info.plist.
        ///
        ///  Returns null if the user is already in the given voice channel.
        ///
        /// </remarks>
        public Call? StartCallWithAudioCallbacks(
            ulong lobbyId,
            UserAudioReceivedCallback receivedCb,
            UserAudioCapturedCallback capturedCb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.Call();
                Call? __returnValue = null;
                Discord.Sdk.NativeMethods.Client.UserAudioReceivedCallback __receivedCbDelegate =
                    NativeMethods.Client.UserAudioReceivedCallback_Handler;
                Discord.Sdk.NativeMethods.Client.UserAudioCapturedCallback __capturedCbDelegate =
                    NativeMethods.Client.UserAudioCapturedCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.Client.StartCallWithAudioCallbacks(
                        self,
                        lobbyId,
                        __receivedCbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(receivedCb),
                        __capturedCbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(capturedCb),
                        &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new Call(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  This will abort the authorize flow if it is in progress and tear down any associated state.
        /// </summary>
        /// <remarks>
        ///  NOTE: this *will not* close authorization windows presented to the user.
        ///
        /// </remarks>
        public void AbortAuthorize()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.AbortAuthorize(self);
                }
            }
        }

        /// <summary>
        ///  This function is used to abort/cleanup the device authorization flow.
        /// </summary>
        public void AbortGetTokenFromDevice()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.AbortGetTokenFromDevice(self);
                }
            }
        }

        /// <summary>
        ///  Initiates an OAuth2 flow for a user to "sign in with Discord". This flow is intended for
        ///  desktop and mobile devices. If you are implementing for the console, leverage the device
        ///  auth flow instead (Client::GetTokenFromDevice or Client::OpenAuthorizeDeviceScreen).
        /// </summary>
        /// <remarks>
        ///  ## Overview
        ///  If you're not familiar with OAuth2, some basic background: At a high level the goal of
        ///  OAuth2 is to allow a user to connect two applications together and share data between them.
        ///  In this case, allowing a game to access some of their Discord data. The high level flow is:
        ///  - This function, Authorize, is invoked to start the OAuth2 process, and the user is sent to
        ///  Discord
        ///  - On Discord, the user sees a prompt to authorize the connection, and that prompt explains
        ///  what data and functionality the game is requesting.
        ///  - Once the user approves the connection, they are redirected back to your application with
        ///  a secret code.
        ///  - You can then exchange that secret code to get back an access token which can be used to
        ///  authenticate with the SDK.
        ///
        ///  ## Public vs Confidential Clients
        ///  Normal OAuth2 requires a backend server to handle exchanging the "code" for a "token" (the
        ///  last step mentioned above). Not all games have backend servers or their own identity system
        ///  though, and for early testing of the SDK that can take some time to setup.
        ///
        ///  If desired, you can instead change your Discord application in the developer portal (on the
        ///  OAuth2 tab), to be a "public" client. This will allow you to exchange the code for a token
        ///  without a backend server, by using the GetToken function below. You can also change this
        ///  setting back once you have a backend in place later too.
        ///
        ///  ## Overlay
        ///  To streamline the authentication process, the SDK will attempt to use the Discord overlay
        ///  if it is enabled. This will allow the user to authenticate without leaving the game,
        ///  enabling a more seamless experience.
        ///
        ///  You should check to see if the Discord overlay works with your game before shipping. It's
        ///  ok if it doesn't though, the SDK will fall back to using a browser window. Once you're
        ///  ready to ship, you can work with us to have the overlay enabled by default for your game
        ///  too.
        ///
        ///  If your game's main window is not the same process that the SDK is running in, then you
        ///  need to tell the SDK the PID of the window that the overlay should attach to. You can do
        ///  this by calling Client::SetGameWindowPid.
        ///
        ///  ## Redirects
        ///  For the Authorize function to work, you must configure a redirect url in your Discord
        ///  application in the developer portal, (it is located on the OAuth2 tab).
        ///  - For desktop applications, add `http://127.0.0.1/callback`
        ///  - For mobile applications, add `discord-APP_ID:/authorize/callback`
        ///
        ///  The SDK will then spin up a local webserver to handle the OAuth2 redirects for you as
        ///  well to streamline your integration.
        ///
        ///  ## Security
        ///  This function accepts an args object, and two of those values are important for security:
        ///  - To prevent CSRF attacks during auth, the SDK automatically attaches a state and checks it
        ///  for you when performing the authorization. You can override state if you want for your own
        ///  flow, but please be mindful to keep it a secure, random value.
        ///  - If you are using the Client::GetToken function you will need to provide a "code
        ///  challenge" or "code verifier". We'll spare you the boring details of how that works (woo...
        ///  crypto), as we've made a simple function to create these for you,
        ///  Client::CreateAuthorizationCodeVerifier. That returns a struct with two items, a
        ///  `challenge` value to pass into this function and a `verifier` value to pass into
        ///  Client::GetToken.
        ///
        ///  ## Callbacks & Code Exchange
        ///  When this flow completes, the given callback function will be invoked with a "code". That
        ///  code must be exchanged for an actual authorization token before it can be used. To start,
        ///  you can use the Client::GetToken function to perform this exchange. Longer term private
        ///  apps will want to move to the server side API for this, since that enables provisional
        ///  accounts to "upgrade" to full Discord accounts.
        ///
        ///  ## Android
        ///  You must add the appropriate intent filter to your `AndroidManifest.xml`.
        ///  `AndroidBuildPostProcessor` in the sample demonstrates how to do this automatically.
        ///
        ///  If you'd like to manage it yourself, the required entry in your `<application>` looks like
        ///  this:
        ///  ```xml
        ///  <activity android:name="com.discord.socialsdk.AuthenticationActivity"
        ///  android:exported="true">
        ///    <intent-filter>
        ///      <action android:name="android.intent.action.VIEW" />
        ///      <category android:name="android.intent.category.DEFAULT" />
        ///      <category android:name="android.intent.category.BROWSABLE" />
        ///      <data android:scheme="discord-1234567890123456789" />
        ///    </intent-filter>
        ///  </activity>
        ///  ```
        ///  Replace the numbers after `discord-` with your Application ID from the Discord developer
        ///  portal.
        ///
        ///  Android support (specifically the builtin auth flow) requires the androidx.browser library
        ///  as a dependency of your app. The sample uses Google External Dependency Manager to add this
        ///  to the Gradle build for the project, but you may use any means of your choosing to add this
        ///  dependency. We currently depend on version 1.8.0 of this library.
        ///
        ///  For more information see: https://discord.com/developers/docs/topics/oauth2
        ///
        /// </remarks>
        public void Authorize(AuthorizationArgs args,
                              AuthorizationCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.AuthorizationArgs* __argsFixed = &args.self)
                {
                    Discord.Sdk.NativeMethods.Client.AuthorizationCallback __callbackDelegate =
                        NativeMethods.Client.AuthorizationCallback_Handler;

                    fixed (NativeMethods.Client* self = &this.self)
                    {
                        NativeMethods.Client.Authorize(
                            self,
                            __argsFixed,
                            __callbackDelegate,
                            NativeMethods.ManagedUserData.Free,
                            NativeMethods.ManagedUserData.CreateHandle(callback));
                    }
                }
            }
        }

        /// <summary>
        ///  This function is used to hide the device authorization screen and is used for the case
        ///  where the user is on a limited input device, such as a console or smart TV. This function
        ///  should be used in conjunction with a backend server to handle the device authorization
        ///  flow. For a public client, you can use Client::AbortGetTokenFromDevice instead.
        /// </summary>
        public void CloseAuthorizeDeviceScreen()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.CloseAuthorizeDeviceScreen(self);
                }
            }
        }

        /// <summary>
        ///  Helper function that can create a code challenge and verifier for use in the
        ///  Client::Authorize + Client::GetToken flow. This returns a struct with two items, a
        ///  `challenge` value to pass into Client::Authorize and a `verifier` value to pass into
        ///  GetToken.
        /// </summary>
        public AuthorizationCodeVerifier CreateAuthorizationCodeVerifier()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __returnValueNative = new NativeMethods.AuthorizationCodeVerifier();
                AuthorizationCodeVerifier? __returnValue = null;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.CreateAuthorizationCodeVerifier(self, &__returnValueNative);
                }

                __returnValue = new AuthorizationCodeVerifier(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Exchanges a parent application token for a child application token.
        /// </summary>
        /// <remarks>
        ///  This is used to get a token for a child application that is linked to the parent
        ///  application. This is only relevant if you have an applications set up in a parent/child
        ///  relationship, which is applicable if you are a publisher with multiple games under the
        ///  same account system. Access to this feature is currently limited.
        ///
        ///  NOTE: This function only works for public clients. Public clients are ones that do not have
        ///  a backend server or their own concept of user accounts and simply rely on a separate system
        ///  for authentication like Steam/Epic.
        ///
        ///  When first testing the SDK, it can be a lot easier to use a public client to get a proof of
        ///  concept going, and change it to a confidential client later. You can toggle that setting on
        ///  the OAuth2 page for your application in the Discord developer portal,
        ///  https://discord.com/developers/applications
        ///
        /// </remarks>
        public void ExchangeChildToken(string parentApplicationToken,
                                       ulong childApplicationId,
                                       ExchangeChildTokenCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __parentApplicationTokenSpan;
                var __parentApplicationTokenOwned =
                    NativeMethods.__InitStringLocal(__scratch,
                        &__scratchUsed,
                        1024,
                        &__parentApplicationTokenSpan,
                        parentApplicationToken);
                Discord.Sdk.NativeMethods.Client.ExchangeChildTokenCallback __callbackDelegate =
                    NativeMethods.Client.ExchangeChildTokenCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.ExchangeChildToken(
                        self,
                        __parentApplicationTokenSpan,
                        childApplicationId,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }

                NativeMethods.__FreeLocalString(&__parentApplicationTokenSpan,
                    __parentApplicationTokenOwned);
            }
        }

        /// <summary>
        ///  Fetches basic information about the user associated with the given auth token.
        /// </summary>
        /// <remarks>
        ///  This can allow you to check if an auth token is valid or not.
        ///  This does not require the client to be connected or to have it's own authentication token,
        ///  so it can be called immediately after the client connects.
        ///
        /// </remarks>
        public void FetchCurrentUser(AuthorizationTokenType tokenType,
                                     string token,
                                     FetchCurrentUserCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __tokenSpan;
                var __tokenOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__tokenSpan, token);
                Discord.Sdk.NativeMethods.Client.FetchCurrentUserCallback __callbackDelegate =
                    NativeMethods.Client.FetchCurrentUserCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.FetchCurrentUser(
                        self,
                        tokenType,
                        __tokenSpan,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }

                NativeMethods.__FreeLocalString(&__tokenSpan, __tokenOwned);
            }
        }

        /// <summary>
        ///  Provisional accounts are a way for users that have not signed up for Discord to still
        ///  access SDK functionality. They are "placeholder" Discord accounts for the user that are
        ///  owned and managed by your game. Provisional accounts exist so that your users can engage
        ///  with Discord APIs and systems without the friction of creating their own Discord account.
        ///  Provisional accounts and their data are unique per Discord application.
        /// </summary>
        /// <remarks>
        ///  This function generates a Discord access token. You pass in the "identity" of the user, and
        ///  it generates a new Discord account that is tied to that identity. There are multiple ways
        ///  of specifying that identity, including using Steam/Epic services, or using your own
        ///  identity system.
        ///
        ///  The callback function will be invoked with an access token that expires in 1 hour. Refresh
        ///  tokens are not supported for provisional accounts, so that will be an empty string. You
        ///  will need to call this function again to get a new access token when the old one expires.
        ///
        ///  NOTE: When the token expires the SDK will still continue to receive updates such as new
        ///  messages sent in a lobby, and any voice calls will continue to be active. But any new
        ///  actions taken will fail such as sending a messaging or adding a friend. You can get a new
        ///  token and pass it to UpdateToken without interrupting the user's experience.
        ///
        ///  It is suggested that these provisional tokens are not stored, and instead to just invoke
        ///  this function each time the game is launched and when these tokens are about to expire.
        ///  However, should you choose to store it, it is recommended to differentiate these
        ///  provisional account tokens from "full" Discord account tokens.
        ///
        ///  NOTE: This function only works for public clients. Public clients are ones that do not have
        ///  a backend server or their own concept of user accounts and simply rely on a separate system
        ///  for authentication like Steam/Epic.
        ///
        ///  When first testing the SDK, it can be a lot easier to use a public client to get a proof of
        ///  concept going, and change it to a confidential client later. You can toggle that setting on
        ///  the OAuth2 page for your application in the Discord developer portal,
        ///  https://discord.com/developers/applications
        ///
        /// </remarks>
        public void GetProvisionalToken(ulong applicationId,
                                        AuthenticationExternalAuthType externalAuthType,
                                        string externalAuthToken,
                                        TokenExchangeCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __externalAuthTokenSpan;
                var __externalAuthTokenOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__externalAuthTokenSpan, externalAuthToken);
                Discord.Sdk.NativeMethods.Client.TokenExchangeCallback __callbackDelegate =
                    NativeMethods.Client.TokenExchangeCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetProvisionalToken(
                        self,
                        applicationId,
                        externalAuthType,
                        __externalAuthTokenSpan,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }

                NativeMethods.__FreeLocalString(&__externalAuthTokenSpan, __externalAuthTokenOwned);
            }
        }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    /// <summary>
    ///  DEBUG ONLY: Creates a provisional token using Discord Bot API.
    ///  This method is only available in debug builds and should not be used in production.
    /// </summary>
    public void GetProvisionalTokenBotAPI(ulong applicationId,
                                          string botToken,
                                          string externalUserId,
                                          string preferredGlobalName,
                                          Discord.Sdk.Client.TokenExchangeCallback callback) {
        if (disposed_ != 0) {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe {
            var __scratchAligned = stackalloc ulong[128];
            var __scratch = (byte*)__scratchAligned;
            var __scratchUsed = 0;
            NativeMethods.Discord_String __botTokenSpan;
            var __botTokenOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__botTokenSpan, botToken);
            NativeMethods.Discord_String __externalUserIdSpan;
            var __externalUserIdOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__externalUserIdSpan, externalUserId);
            NativeMethods.Discord_String __preferredGlobalNameSpan;
            var __preferredGlobalNameOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__preferredGlobalNameSpan, preferredGlobalName);
            Discord.Sdk.NativeMethods.Client.TokenExchangeCallback __callbackDelegate =
              Discord.Sdk.NativeMethods.Client.TokenExchangeCallback_Handler;
            fixed(NativeMethods.Client* self = &this.self) {
                NativeMethods.Client.GetProvisionalTokenBotAPI(
                  self,
                  applicationId,
                  __botTokenSpan,
                  __externalUserIdSpan,
                  __preferredGlobalNameSpan,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
            NativeMethods.__FreeLocalString(&__preferredGlobalNameSpan, __preferredGlobalNameOwned);
            NativeMethods.__FreeLocalString(&__externalUserIdSpan, __externalUserIdOwned);
            NativeMethods.__FreeLocalString(&__botTokenSpan, __botTokenOwned);
        }
    }
#endif
        /// <summary>
        ///  Exchanges an authorization code that was returned from the Client::Authorize function
        ///  for an access token which can be used to authenticate with the SDK.
        /// </summary>
        /// <remarks>
        ///  The callback function will be invoked with two tokens:
        ///  - An access token which can be used to authenticate with the SDK, but expires after 7 days.
        ///  - A refresh token, which cannot be used to authenticate, but can be used to get a new
        ///  access token later. Refresh tokens do not currently expire.
        ///
        ///  It will also include when the access token expires in seconds.
        ///  You will want to store this value as well and refresh the token when it gets close to
        ///  expiring (for example if the user launches the game and the token expires within 24 hours,
        ///  it would be good to refresh it).
        ///
        ///  For more information see https://discord.com/developers/docs/topics/oauth2
        ///
        ///  NOTE: This function only works for public clients. Public clients are ones that do not have
        ///  a backend server or their own concept of user accounts and simply rely on a separate system
        ///  for authentication like Steam/Epic.
        ///
        ///  When first testing the SDK, it can be a lot easier to use a public client to get a proof of
        ///  concept going, and change it to a confidential client later. You can toggle that setting on
        ///  the OAuth2 page for your application in the Discord developer portal,
        ///  https://discord.com/developers/applications
        ///
        /// </remarks>
        public void GetToken(ulong applicationId,
                             string code,
                             string codeVerifier,
                             string redirectUri,
                             TokenExchangeCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __codeSpan;
                var __codeOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__codeSpan, code);
                NativeMethods.Discord_String __codeVerifierSpan;
                var __codeVerifierOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__codeVerifierSpan, codeVerifier);
                NativeMethods.Discord_String __redirectUriSpan;
                var __redirectUriOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__redirectUriSpan, redirectUri);
                Discord.Sdk.NativeMethods.Client.TokenExchangeCallback __callbackDelegate =
                    NativeMethods.Client.TokenExchangeCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetToken(self,
                        applicationId,
                        __codeSpan,
                        __codeVerifierSpan,
                        __redirectUriSpan,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }

                NativeMethods.__FreeLocalString(&__redirectUriSpan, __redirectUriOwned);
                NativeMethods.__FreeLocalString(&__codeVerifierSpan, __codeVerifierOwned);
                NativeMethods.__FreeLocalString(&__codeSpan, __codeOwned);
            }
        }

        /// <summary>
        ///  This function is a combination of Client::Authorize and Client::GetToken, but is used for
        ///  the case where the user is on a limited input device, such as a console or smart TV.
        /// </summary>
        /// <remarks>
        ///  The callback function will be invoked with two tokens:
        ///  - An access token which can be used to authenticate with the SDK, but expires after 7 days.
        ///  - A refresh token, which cannot be used to authenticate, but can be used to get a new
        ///  access token later. Refresh tokens do not currently expire.
        ///
        ///  It will also include when the access token expires in seconds.
        ///  You will want to store this value as well and refresh the token when it gets close to
        ///  expiring (for example if the user launches the game and the token expires within 24 hours,
        ///  it would be good to refresh it).
        ///
        ///  For more information see https://discord.com/developers/docs/topics/oauth2
        ///
        ///  NOTE: This function only works for public clients. Public clients are ones that do not have
        ///  a backend server or their own concept of user accounts and simply rely on a separate system
        ///  for authentication like Steam/Epic. If you have a backend server for auth, you can use
        ///  Client::OpenAuthorizeDeviceScreen and Client::CloseAuthorizeDeviceScreen to show/hide the
        ///  UI for the device auth flow.
        ///
        ///  When first testing the SDK, it can be a lot easier to use a public client to get a proof of
        ///  concept going, and change it to a confidential client later. You can toggle that setting on
        ///  the OAuth2 page for your application in the Discord developer portal,
        ///  https://discord.com/developers/applications
        ///
        /// </remarks>
        public void GetTokenFromDevice(DeviceAuthorizationArgs args,
                                       TokenExchangeCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.DeviceAuthorizationArgs* __argsFixed = &args.self)
                {
                    Discord.Sdk.NativeMethods.Client.TokenExchangeCallback __callbackDelegate =
                        NativeMethods.Client.TokenExchangeCallback_Handler;

                    fixed (NativeMethods.Client* self = &this.self)
                    {
                        NativeMethods.Client.GetTokenFromDevice(
                            self,
                            __argsFixed,
                            __callbackDelegate,
                            NativeMethods.ManagedUserData.Free,
                            NativeMethods.ManagedUserData.CreateHandle(callback));
                    }
                }
            }
        }

        /// <summary>
        ///  This function is a combination of Client::Authorize and
        ///  Client::GetTokenFromProvisionalMerge, but is used for the case where the user is on a
        ///  limited input device, such as a console or smart TV.
        /// </summary>
        /// <remarks>
        ///  This function should be used whenever a user with a provisional account wants to link to an
        ///  existing Discord account or "upgrade" their provisional account into a "full" Discord
        ///  account.
        ///
        ///  In this case, data from the provisional account should be "migrated" to the Discord
        ///  account, a process we call "account merging". Specifically relationships, DMs, and lobby
        ///  memberships are transferred to the Discord account.
        ///
        ///  The provisional account will be deleted once this merging process completes. If the user
        ///  later unlinks, then a new provisional account with a new unique ID is created.
        ///
        ///  The account merging process starts the same as the normal login flow, by invoking the
        ///  GetTokenFromDevice. But instead of calling GetTokenFromDevice, call this function and pass
        ///  in the provisional user's identity as well.
        ///
        ///  The Discord backend can then find both the provisional account with that identity and the
        ///  new Discord account and merge any data as necessary.
        ///
        ///  See the documentation for GetTokenFromDevice for more details on the callback. Note that
        ///  the callback will be invoked when the token exchange completes, but the process of merging
        ///  accounts happens asynchronously so will not be complete yet.
        ///
        ///  NOTE: This function only works for public clients. Public clients are ones that do not have
        ///  a backend server or their own concept of user accounts and simply rely on a separate system
        ///  for authentication like Steam/Epic. If you have a backend server for auth, you can use
        ///  Client::OpenAuthorizeDeviceScreen and Client::CloseAuthorizeDeviceScreen to show/hide the
        ///  UI for the device auth flow.
        ///
        ///  When first testing the SDK, it can be a lot easier to use a public client to get a proof of
        ///  concept going, and change it to a confidential client later. You can toggle that setting on
        ///  the OAuth2 page for your application in the Discord developer portal,
        ///  https://discord.com/developers/applications
        ///
        /// </remarks>
        public void GetTokenFromDeviceProvisionalMerge(
            DeviceAuthorizationArgs args,
            AuthenticationExternalAuthType externalAuthType,
            string externalAuthToken,
            TokenExchangeCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.DeviceAuthorizationArgs* __argsFixed = &args.self)
                {
                    var __scratchAligned = stackalloc ulong[128];
                    var __scratch = (byte*)__scratchAligned;
                    var __scratchUsed = 0;
                    NativeMethods.Discord_String __externalAuthTokenSpan;
                    var __externalAuthTokenOwned = NativeMethods.__InitStringLocal(
                        __scratch, &__scratchUsed, 1024, &__externalAuthTokenSpan, externalAuthToken);
                    Discord.Sdk.NativeMethods.Client.TokenExchangeCallback __callbackDelegate =
                        NativeMethods.Client.TokenExchangeCallback_Handler;

                    fixed (NativeMethods.Client* self = &this.self)
                    {
                        NativeMethods.Client.GetTokenFromDeviceProvisionalMerge(
                            self,
                            __argsFixed,
                            externalAuthType,
                            __externalAuthTokenSpan,
                            __callbackDelegate,
                            NativeMethods.ManagedUserData.Free,
                            NativeMethods.ManagedUserData.CreateHandle(callback));
                    }

                    NativeMethods.__FreeLocalString(&__externalAuthTokenSpan, __externalAuthTokenOwned);
                }
            }
        }

        /// <summary>
        ///  This function should be used with the Client::Authorize function whenever a user with a
        ///  provisional account wants to link to an existing Discord account or "upgrade" their
        ///  provisional account into a "full" Discord account.
        /// </summary>
        /// <remarks>
        ///  In this case, data from the provisional account should be "migrated" to the Discord
        ///  account, a process we call "account merging". Specifically relationships, DMs, and lobby
        ///  memberships are transferred to the Discord account.
        ///
        ///  The provisional account will be deleted once this merging process completes. If the user
        ///  later unlinks, then a new provisional account with a new unique ID is created.
        ///
        ///  The account merging process starts the same as the normal login flow, by invoking the
        ///  Authorize method to get an authorization code back. But instead of calling GetToken, call
        ///  this function and pass in the provisional user's identity as well.
        ///
        ///  The Discord backend can then find both the provisional account with that identity and the
        ///  new Discord account and merge any data as necessary.
        ///
        ///  See the documentation for GetToken for more details on the callback. Note that the callback
        ///  will be invoked when the token exchange completes, but the process of merging accounts
        ///  happens asynchronously so will not be complete yet.
        ///
        ///  NOTE: This function only works for public clients. Public clients are ones that do not have
        ///  a backend server or their own concept of user accounts and simply rely on a separate system
        ///  for authentication like Steam/Epic.
        ///
        ///  When first testing the SDK, it can be a lot easier to use a public client to get a proof of
        ///  concept going, and change it to a confidential client later. You can toggle that setting on
        ///  the OAuth2 page for your application in the Discord developer portal,
        ///  https://discord.com/developers/applications
        ///
        /// </remarks>
        public void GetTokenFromProvisionalMerge(
            ulong applicationId,
            string code,
            string codeVerifier,
            string redirectUri,
            AuthenticationExternalAuthType externalAuthType,
            string externalAuthToken,
            TokenExchangeCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __codeSpan;
                var __codeOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__codeSpan, code);
                NativeMethods.Discord_String __codeVerifierSpan;
                var __codeVerifierOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__codeVerifierSpan, codeVerifier);
                NativeMethods.Discord_String __redirectUriSpan;
                var __redirectUriOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__redirectUriSpan, redirectUri);
                NativeMethods.Discord_String __externalAuthTokenSpan;
                var __externalAuthTokenOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__externalAuthTokenSpan, externalAuthToken);
                Discord.Sdk.NativeMethods.Client.TokenExchangeCallback __callbackDelegate =
                    NativeMethods.Client.TokenExchangeCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetTokenFromProvisionalMerge(
                        self,
                        applicationId,
                        __codeSpan,
                        __codeVerifierSpan,
                        __redirectUriSpan,
                        externalAuthType,
                        __externalAuthTokenSpan,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }

                NativeMethods.__FreeLocalString(&__externalAuthTokenSpan, __externalAuthTokenOwned);
                NativeMethods.__FreeLocalString(&__redirectUriSpan, __redirectUriOwned);
                NativeMethods.__FreeLocalString(&__codeVerifierSpan, __codeVerifierOwned);
                NativeMethods.__FreeLocalString(&__codeSpan, __codeOwned);
            }
        }

        /// <summary>
        ///  Returns true if the SDK has a non-empty OAuth2 token set, regardless of whether that token
        ///  is valid or not.
        /// </summary>
        public bool IsAuthenticated()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnValue = NativeMethods.Client.IsAuthenticated(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  This function is used to show the device authorization screen and is used for the case
        ///  where the user is on a limited input device, such as a console or smart TV. This function
        ///  should be used in conjunction with a backend server to handle the device authorization
        ///  flow. For a public client, you can use Client::GetTokenFromDevice instead.
        /// </summary>
        public void OpenAuthorizeDeviceScreen(ulong clientId, string userCode)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __userCodeSpan;
                var __userCodeOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__userCodeSpan, userCode);

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.OpenAuthorizeDeviceScreen(self, clientId, __userCodeSpan);
                }

                NativeMethods.__FreeLocalString(&__userCodeSpan, __userCodeOwned);
            }
        }

        /// <summary>
        ///  Some functions don't work for provisional accounts, and require the user
        ///  merge their account into a full Discord account before proceeding. This
        ///  callback is invoked when an account merge must take place before
        ///  proceeding. The developer is responsible for initiating the account merge,
        ///  and then calling Client::ProvisionalUserMergeCompleted to signal to the SDK that
        ///  the pending operation can continue with the new account.
        /// </summary>
        public void ProvisionalUserMergeCompleted(bool success)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.ProvisionalUserMergeCompleted(self, success);
                }
            }
        }

        /// <summary>
        ///  Generates a new access token for the current user from a refresh token.
        /// </summary>
        /// <remarks>
        ///  Once this is called, the old access and refresh tokens are both invalidated and cannot be
        ///  used again. The callback function will be invoked with a new access and refresh token. See
        ///  GetToken for more details.
        ///
        ///  NOTE: This function only works for public clients. Public clients are ones that do not have
        ///  a backend server or their own concept of user accounts and simply rely on a separate system
        ///  for authentication like Steam/Epic.
        ///
        ///  When first testing the SDK, it can be a lot easier to use a public client to get a proof of
        ///  concept going, and change it to a confidential client later. You can toggle that setting on
        ///  the OAuth2 page for your application in the Discord developer portal,
        ///  https://discord.com/developers/applications
        ///
        /// </remarks>
        public void RefreshToken(ulong applicationId,
                                 string refreshToken,
                                 TokenExchangeCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __refreshTokenSpan;
                var __refreshTokenOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__refreshTokenSpan, refreshToken);
                Discord.Sdk.NativeMethods.Client.TokenExchangeCallback __callbackDelegate =
                    NativeMethods.Client.TokenExchangeCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.RefreshToken(
                        self,
                        applicationId,
                        __refreshTokenSpan,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }

                NativeMethods.__FreeLocalString(&__refreshTokenSpan, __refreshTokenOwned);
            }
        }

        /// <summary>
        ///  Registers a callback to be invoked when a user requests to initiate the authorization flow.
        /// </summary>
        /// <remarks>
        ///  When you register this callback, the Discord app will show new entry points to allow users
        ///  to initiate the authorization flow.
        ///
        ///  This function is tied to upcoming Discord client functionality experiments that will be
        ///  rolled out to a percentage of Discord users over time. More documentation and
        ///  implementation details to come as the client experiments run.
        ///
        /// </remarks>
        public void RegisterAuthorizeRequestCallback(
            AuthorizeRequestCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.AuthorizeRequestCallback __callbackDelegate =
                    NativeMethods.Client.AuthorizeRequestCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.RegisterAuthorizeRequestCallback(
                        self,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }

        /// <summary>
        ///  Stops listening for the AUTHORIZE_REQUEST event and removes the registered callback
        /// </summary>
        /// <remarks>
        ///  This function is tied to upcoming Discord client functionality experiments that will be
        ///  rolled out to a percentage of Discord users over time. More documentation and
        ///  implementation details to come as the client experiments run.
        ///
        /// </remarks>
        public void RemoveAuthorizeRequestCallback()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.RemoveAuthorizeRequestCallback(self);
                }
            }
        }

        /// <summary>
        ///  Revoke all application access/refresh tokens associated with a user with any valid
        ///  access/refresh token. This will invalidate all tokens and they cannot be used again. This
        ///  is useful if you want to log the user out of the game and invalidate their session.
        /// </summary>
        /// <remarks>
        ///  NOTE: This function only works for public clients. Public clients are ones that do not have
        ///  a backend server or their own concept of user accounts and simply rely on a separate system
        ///  for authentication like Steam/Epic.
        ///
        ///  When first testing the SDK, it can be a lot easier to use a public client to get a proof of
        ///  concept going, and change it to a confidential client later. You can toggle that setting on
        ///  the OAuth2 page for your application in the Discord developer portal,
        ///  https://discord.com/developers/applications
        ///
        /// </remarks>
        public void RevokeToken(ulong applicationId,
                                string token,
                                RevokeTokenCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __tokenSpan;
                var __tokenOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__tokenSpan, token);
                Discord.Sdk.NativeMethods.Client.RevokeTokenCallback __callbackDelegate =
                    NativeMethods.Client.RevokeTokenCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.RevokeToken(
                        self,
                        applicationId,
                        __tokenSpan,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }

                NativeMethods.__FreeLocalString(&__tokenSpan, __tokenOwned);
            }
        }

        /// <summary>
        ///  Sets a callback function to be invoked when the device authorization screen is closed.
        /// </summary>
        public void SetAuthorizeDeviceScreenClosedCallback(
            AuthorizeDeviceScreenClosedCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.AuthorizeDeviceScreenClosedCallback __cbDelegate =
                    NativeMethods.Client.AuthorizeDeviceScreenClosedCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetAuthorizeDeviceScreenClosedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  When users are linking their account with Discord, which involves an OAuth2 flow,
        ///  the SDK can streamline it by using Discord's overlay so the interaction happens entirely
        ///  in-game. If your game's main window is not the same process as the one running the
        ///  integration you may need to set the window PID using this method. It defaults to the
        ///  current pid.
        /// </summary>
        public void SetGameWindowPid(int pid)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetGameWindowPid(self, pid);
                }
            }
        }

        /// <summary>
        ///  Get a notification when the current token is about to expire or expired.
        /// </summary>
        /// <remarks>
        ///  This callback is invoked when the SDK detects that the last token passed to
        ///  Client::UpdateToken is nearing expiration or has expired. This is a signal to the developer
        ///  to refresh the token. The callback is invoked once per token, and will not be invoked again
        ///  until a new token is passed to Client::UpdateToken.
        ///
        ///  If the token is refreshed before the expiration callback is invoked, call
        ///  Client::UpdateToken to pass in the new token and reconfigure the token expiration.
        ///
        ///  If your client is disconnected (the token was expired when connecting or was revoked while
        ///  connected), the expiration callback will not be invoked.
        ///
        /// </remarks>
        public void SetTokenExpirationCallback(TokenExpirationCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.TokenExpirationCallback __callbackDelegate =
                    NativeMethods.Client.TokenExpirationCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetTokenExpirationCallback(
                        self,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }

        /// <summary>
        ///  This function is used to unlink/unmerge a external identity from a Discord account. This
        ///  is useful if the user wants to unlink their external identity from their Discord account
        ///  and create a new provisional account for that identity. This will invalidate all
        ///  access/refresh tokens for the user and they cannot be used again.
        /// </summary>
        /// <remarks>
        ///  This function should be used with the Client::GetProvisionalToken function to get a
        ///  provisional token for the newly created provisional account.
        ///
        ///  NOTE: This function only works for public clients. Public clients are ones that do not have
        ///  a backend server or their own concept of user accounts and simply rely on a separate system
        ///  for authentication like Steam/Epic.
        ///
        ///  When first testing the SDK, it can be a lot easier to use a public client to get a proof of
        ///  concept going, and change it to a confidential client later. You can toggle that setting on
        ///  the OAuth2 page for your application in the Discord developer portal,
        ///  https://discord.com/developers/applications
        ///
        /// </remarks>
        public void UnmergeIntoProvisionalAccount(
            ulong applicationId,
            AuthenticationExternalAuthType externalAuthType,
            string externalAuthToken,
            UnmergeIntoProvisionalAccountCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __externalAuthTokenSpan;
                var __externalAuthTokenOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__externalAuthTokenSpan, externalAuthToken);
                Discord.Sdk.NativeMethods.Client
                    .UnmergeIntoProvisionalAccountCallback __callbackDelegate =
                        NativeMethods.Client.UnmergeIntoProvisionalAccountCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.UnmergeIntoProvisionalAccount(
                        self,
                        applicationId,
                        externalAuthType,
                        __externalAuthTokenSpan,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }

                NativeMethods.__FreeLocalString(&__externalAuthTokenSpan, __externalAuthTokenOwned);
            }
        }

        /// <summary>
        ///  Updates the display name of a provisional account to the specified name.
        /// </summary>
        /// <remarks>
        ///  This should generally be invoked whenever the SDK starts and whenever a provisional account
        ///  changes their name, since the auto-generated name for provisional accounts is just a random
        ///  string.
        ///
        /// </remarks>
        public void UpdateProvisionalAccountDisplayName(
            string name,
            UpdateProvisionalAccountDisplayNameCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __nameSpan;
                var __nameOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__nameSpan, name);
                Discord.Sdk.NativeMethods.Client
                    .UpdateProvisionalAccountDisplayNameCallback __callbackDelegate =
                        NativeMethods.Client.UpdateProvisionalAccountDisplayNameCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.UpdateProvisionalAccountDisplayName(
                        self,
                        __nameSpan,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }

                NativeMethods.__FreeLocalString(&__nameSpan, __nameOwned);
            }
        }

        /// <summary>
        ///  Asynchronously sets a new auth token for this client to use.
        /// </summary>
        /// <remarks>
        ///  If your client is already connected, this function *may* trigger a reconnect.
        ///  If your client is not connected, this function will only update the auth token, and so you
        ///  must invoke Client::Connect as well. You should wait for the given callback function to be
        ///  invoked though so that the next Client::Connect attempt uses the updated token.
        ///
        /// </remarks>
        public void UpdateToken(AuthorizationTokenType tokenType,
                                string token,
                                UpdateTokenCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __tokenSpan;
                var __tokenOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__tokenSpan, token);
                Discord.Sdk.NativeMethods.Client.UpdateTokenCallback __callbackDelegate =
                    NativeMethods.Client.UpdateTokenCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.UpdateToken(
                        self,
                        tokenType,
                        __tokenSpan,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }

                NativeMethods.__FreeLocalString(&__tokenSpan, __tokenOwned);
            }
        }

        /// <summary>
        ///  Returns true if the given message is able to be viewed in a Discord client.
        /// </summary>
        /// <remarks>
        ///  Not all chat messages are replicated to Discord. For example lobby chat and some DMs
        ///  are ephemeral and not persisted on Discord so cannot be opened. This function checks those
        ///  conditions and makes sure the message is viewable in Discord.
        ///
        /// </remarks>
        public bool CanOpenMessageInDiscord(ulong messageId)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnValue = NativeMethods.Client.CanOpenMessageInDiscord(self, messageId);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Deletes the specified message sent by the current user to the specified recipient.
        /// </summary>
        public void DeleteUserMessage(ulong recipientId,
                                      ulong messageId,
                                      DeleteUserMessageCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.DeleteUserMessageCallback __cbDelegate =
                    NativeMethods.Client.DeleteUserMessageCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.DeleteUserMessage(
                        self,
                        recipientId,
                        messageId,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Edits the specified message sent by the current user to the specified recipient.
        /// </summary>
        /// <remarks>
        ///  All of the same restrictions apply as for sending a message, see SendUserMessage for more.
        ///
        /// </remarks>
        public void EditUserMessage(ulong recipientId,
                                    ulong messageId,
                                    string content,
                                    EditUserMessageCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __contentSpan;
                var __contentOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__contentSpan, content);
                Discord.Sdk.NativeMethods.Client.EditUserMessageCallback __cbDelegate =
                    NativeMethods.Client.EditUserMessageCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.EditUserMessage(
                        self,
                        recipientId,
                        messageId,
                        __contentSpan,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }

                NativeMethods.__FreeLocalString(&__contentSpan, __contentOwned);
            }
        }

        /// <summary>
        ///  Returns a reference to the Discord channel object for the given ID.
        /// </summary>
        /// <remarks>
        ///  All messages in Discord are sent in a channel, so the most common use for this will be
        ///  to look up the channel a message was sent in.
        ///  For convience this API will also work with lobbies, so the three possible return values
        ///  for the SDK are a DM, an Ephemeral DM, and a Lobby.
        ///
        /// </remarks>
        public ChannelHandle? GetChannelHandle(ulong channelId)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.ChannelHandle();
                ChannelHandle? __returnValue = null;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.Client.GetChannelHandle(self, channelId, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new ChannelHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Retrieves recent messages from the specified lobby.
        /// </summary>
        /// <remarks>
        ///  Returns a list of MessageHandle representing the recent messages in the lobby,
        ///  with a maximum of 200 messages and up to 72 hours.
        ///  The messages are returned in reverse chronological order (newest first).
        ///  This function requires the current user to be a member of the lobby.
        ///
        ///  Note: This function makes an HTTP request to Discord's API to retrieve messages, as opposed
        ///  to only returning messages that are cached locally by the SDK.
        ///
        ///  Retrieves recent messages from the specified lobby with the specified limit.
        ///
        /// </remarks>
        public void GetLobbyMessagesWithLimit(ulong lobbyId,
                                              int limit,
                                              GetLobbyMessagesCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.GetLobbyMessagesCallback __cbDelegate =
                    NativeMethods.Client.GetLobbyMessagesCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetLobbyMessagesWithLimit(
                        self,
                        lobbyId,
                        limit,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Returns a reference to the Discord message object for the given ID.
        /// </summary>
        /// <remarks>
        ///  The SDK keeps the 25 most recent messages in each channel in memory.
        ///  Messages sent before the SDK was started cannot be accessed with this.
        ///
        /// </remarks>
        public MessageHandle? GetMessageHandle(ulong messageId)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.MessageHandle();
                MessageHandle? __returnValue = null;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.Client.GetMessageHandle(self, messageId, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new MessageHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Retrieves message conversation summaries for all users the current user has DM
        ///  conversations with.
        /// </summary>
        /// <remarks>
        ///  The callback will be invoked with a list of UserMessageSummary objects containing:
        ///  - userId: The ID of the user this conversation is with
        ///  - lastMessageId: The ID of the most recent message in this conversation
        ///
        /// </remarks>
        public void GetUserMessageSummaries(UserMessageSummariesCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.UserMessageSummariesCallback __cbDelegate =
                    NativeMethods.Client.UserMessageSummariesCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetUserMessageSummaries(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Retrieves messages from the DM conversation with the specified user.
        /// </summary>
        /// <remarks>
        ///  Returns a list of MessageHandle representing the recent messages in the conversation with
        ///  the recipient, with a with a maximum of 200 messages and up to 72 hours. The messages are
        ///  returned in reverse chronological order (newest first). This function checks the local
        ///  cache first and only makes an HTTP request to Discord's API if there are not enough cached
        ///  messages available.
        ///
        ///  If limit is greater than 0, restricts the number of messages returned. If limit is 0
        ///  or negative, the limit parameter is 200 messages and 72 hours. This is intended for
        ///  games to load message history when users open a DM conversation.
        ///
        ///  If either user hasn't played the game, there will be no channel between them and
        ///  this function will return a 404 `discordpp::ErrorType::HTTPError` error.
        ///
        /// </remarks>
        public void GetUserMessagesWithLimit(ulong recipientId,
                                             int limit,
                                             UserMessagesWithLimitCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.UserMessagesWithLimitCallback __cbDelegate =
                    NativeMethods.Client.UserMessagesWithLimitCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetUserMessagesWithLimit(
                        self,
                        recipientId,
                        limit,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Opens the given message in the Discord client.
        /// </summary>
        /// <remarks>
        ///  This is useful when a message is sent that contains content that cannot be viewed in
        ///  Discord. You can call this function in the click handler for any CTA you show to view the
        ///  message in Discord.
        ///
        /// </remarks>
        public void OpenMessageInDiscord(
            ulong messageId,
            ProvisionalUserMergeRequiredCallback provisionalUserMergeRequiredCallback,
            OpenMessageInDiscordCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client
                    .ProvisionalUserMergeRequiredCallback __provisionalUserMergeRequiredCallbackDelegate =
                        NativeMethods.Client.ProvisionalUserMergeRequiredCallback_Handler;
                Discord.Sdk.NativeMethods.Client.OpenMessageInDiscordCallback __callbackDelegate =
                    NativeMethods.Client.OpenMessageInDiscordCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.OpenMessageInDiscord(
                        self,
                        messageId,
                        __provisionalUserMergeRequiredCallbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(provisionalUserMergeRequiredCallback),
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }

        /// <summary>
        ///  Sends a message in a lobby chat to all members of the lobby.
        /// </summary>
        /// <remarks>
        ///  The content of the message is restricted to 2,000 characters maximum.
        ///  See https://discord.com/developers/docs/resources/message for more details.
        ///
        ///  The content of the message can also contain special markup for formatting if desired, see
        ///  https://discord.com/developers/docs/reference#message-formatting for more details.
        ///
        ///  If the lobby is linked to a channel, the message will also be sent to that channel on
        ///  Discord.
        ///
        /// </remarks>
        public void SendLobbyMessage(ulong lobbyId,
                                     string content,
                                     SendUserMessageCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __contentSpan;
                var __contentOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__contentSpan, content);
                Discord.Sdk.NativeMethods.Client.SendUserMessageCallback __cbDelegate =
                    NativeMethods.Client.SendUserMessageCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SendLobbyMessage(
                        self,
                        lobbyId,
                        __contentSpan,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }

                NativeMethods.__FreeLocalString(&__contentSpan, __contentOwned);
            }
        }

        /// <summary>
        ///  Variant of Client::SendLobbyMessage that also accepts metadata to be sent with the message.
        /// </summary>
        /// <remarks>
        ///  Metadata is just simple string key/value pairs.
        ///  An example use case for this might be to include the name of the character that sent a
        ///  message.
        ///
        /// </remarks>
        public void SendLobbyMessageWithMetadata(ulong lobbyId,
                                                 string content,
                                                 Dictionary<string, string> metadata,
                                                 SendUserMessageCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __contentSpan;
                var __contentOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__contentSpan, content);
                NativeMethods.Discord_Properties __metadataNative;
                __metadataNative.size = (IntPtr)metadata.Count;
                NativeMethods.Discord_String* __metadataKeys;
                NativeMethods.Discord_String* __metadataValues;
                bool* __metadataKeyOwnership;
                bool* __metadataValueOwnership;
                var __metadataKeysOwned = NativeMethods.__AllocLocalStringArray(
                    __scratch, &__scratchUsed, 1024, &__metadataKeys, metadata.Count);
                var __metadataValuesOwned = NativeMethods.__AllocLocalStringArray(
                    __scratch, &__scratchUsed, 1024, &__metadataValues, metadata.Count);
                var __metadataKeyOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
                    __scratch, &__scratchUsed, 1024, &__metadataKeyOwnership, metadata.Count);
                var __metadataValueOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
                    __scratch, &__scratchUsed, 1024, &__metadataValueOwnership, metadata.Count);
                {
                    int __i = 0;

                    foreach (var (__metadataKey, __metadataValue) in metadata)
                    {
                        NativeMethods.Discord_String __metadataKeySpan;
                        NativeMethods.Discord_String __metadataValueSpan;
                        __metadataKeyOwnership[__i] = NativeMethods.__InitStringLocal(
                            __scratch, &__scratchUsed, 1024, &__metadataKeySpan, __metadataKey);
                        __metadataValueOwnership[__i] = NativeMethods.__InitStringLocal(
                            __scratch, &__scratchUsed, 1024, &__metadataValueSpan, __metadataValue);
                        __metadataKeys[__i] = __metadataKeySpan;
                        __metadataValues[__i] = __metadataValueSpan;
                        __i++;
                    }
                }
                __metadataNative.keys = __metadataKeys;
                __metadataNative.values = __metadataValues;
                Discord.Sdk.NativeMethods.Client.SendUserMessageCallback __cbDelegate =
                    NativeMethods.Client.SendUserMessageCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SendLobbyMessageWithMetadata(
                        self,
                        lobbyId,
                        __contentSpan,
                        __metadataNative,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }

                for (int __i = 0; __i < (int)__metadataNative.size; __i++)
                {
                    NativeMethods.__FreeLocalString(&__metadataKeys[__i], __metadataKeyOwnership[__i]);
                    NativeMethods.__FreeLocalString(&__metadataValues[__i],
                        __metadataValueOwnership[__i]);
                }

                NativeMethods.__FreeLocal(__metadataKeys, __metadataKeysOwned);
                NativeMethods.__FreeLocal(__metadataValues, __metadataValuesOwned);
                NativeMethods.__FreeLocalString(&__contentSpan, __contentOwned);
            }
        }

        /// <summary>
        ///  Sends a direct message to the specified user.
        /// </summary>
        /// <remarks>
        ///  The content of the message is restricted to 2,000 characters maximum.
        ///  See https://discord.com/developers/docs/resources/message for more details.
        ///
        ///  The content of the message can also contain special markup for formatting if desired, see
        ///  https://discord.com/developers/docs/reference#message-formatting for more details.
        ///
        ///  A message can be sent between two users in the following situations:
        ///  - Both users are online and in the game and have not blocked each other
        ///  - Both users are friends with each other
        ///  - Both users share a mutual Discord server and have previously DM'd each other on Discord
        ///
        /// </remarks>
        public void SendUserMessage(ulong recipientId,
                                    string content,
                                    SendUserMessageCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __contentSpan;
                var __contentOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__contentSpan, content);
                Discord.Sdk.NativeMethods.Client.SendUserMessageCallback __cbDelegate =
                    NativeMethods.Client.SendUserMessageCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SendUserMessage(
                        self,
                        recipientId,
                        __contentSpan,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }

                NativeMethods.__FreeLocalString(&__contentSpan, __contentOwned);
            }
        }

        /// <summary>
        ///  Variant of Client::SendUserMessage that also accepts metadata to be sent with the message.
        /// </summary>
        /// <remarks>
        ///  Metadata is just simple string key/value pairs.
        ///  An example use case for this might be to include the name of the character that sent a
        ///  message.
        ///
        /// </remarks>
        public void SendUserMessageWithMetadata(ulong recipientId,
                                                string content,
                                                Dictionary<string, string> metadata,
                                                SendUserMessageCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __contentSpan;
                var __contentOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__contentSpan, content);
                NativeMethods.Discord_Properties __metadataNative;
                __metadataNative.size = (IntPtr)metadata.Count;
                NativeMethods.Discord_String* __metadataKeys;
                NativeMethods.Discord_String* __metadataValues;
                bool* __metadataKeyOwnership;
                bool* __metadataValueOwnership;
                var __metadataKeysOwned = NativeMethods.__AllocLocalStringArray(
                    __scratch, &__scratchUsed, 1024, &__metadataKeys, metadata.Count);
                var __metadataValuesOwned = NativeMethods.__AllocLocalStringArray(
                    __scratch, &__scratchUsed, 1024, &__metadataValues, metadata.Count);
                var __metadataKeyOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
                    __scratch, &__scratchUsed, 1024, &__metadataKeyOwnership, metadata.Count);
                var __metadataValueOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
                    __scratch, &__scratchUsed, 1024, &__metadataValueOwnership, metadata.Count);
                {
                    int __i = 0;

                    foreach (var (__metadataKey, __metadataValue) in metadata)
                    {
                        NativeMethods.Discord_String __metadataKeySpan;
                        NativeMethods.Discord_String __metadataValueSpan;
                        __metadataKeyOwnership[__i] = NativeMethods.__InitStringLocal(
                            __scratch, &__scratchUsed, 1024, &__metadataKeySpan, __metadataKey);
                        __metadataValueOwnership[__i] = NativeMethods.__InitStringLocal(
                            __scratch, &__scratchUsed, 1024, &__metadataValueSpan, __metadataValue);
                        __metadataKeys[__i] = __metadataKeySpan;
                        __metadataValues[__i] = __metadataValueSpan;
                        __i++;
                    }
                }
                __metadataNative.keys = __metadataKeys;
                __metadataNative.values = __metadataValues;
                Discord.Sdk.NativeMethods.Client.SendUserMessageCallback __cbDelegate =
                    NativeMethods.Client.SendUserMessageCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SendUserMessageWithMetadata(
                        self,
                        recipientId,
                        __contentSpan,
                        __metadataNative,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }

                for (int __i = 0; __i < (int)__metadataNative.size; __i++)
                {
                    NativeMethods.__FreeLocalString(&__metadataKeys[__i], __metadataKeyOwnership[__i]);
                    NativeMethods.__FreeLocalString(&__metadataValues[__i],
                        __metadataValueOwnership[__i]);
                }

                NativeMethods.__FreeLocal(__metadataKeys, __metadataKeysOwned);
                NativeMethods.__FreeLocal(__metadataValues, __metadataValuesOwned);
                NativeMethods.__FreeLocalString(&__contentSpan, __contentOwned);
            }
        }

        /// <summary>
        ///  Sets a callback to be invoked whenever a new message is received in either a lobby or a DM.
        /// </summary>
        /// <remarks>
        ///  From the messageId you can fetch the MessageHandle and then the ChannelHandle to determine
        ///  the location the message was sent as well.
        ///
        ///  If the user has the Discord desktop application open on the same machine as the game, then
        ///  they will hear notifications from the Discord application, even though they are able to see
        ///  those messages in game. So to avoid double-notifying users, you should call
        ///  Client::SetShowingChat whenever the chat is shown or hidden to suppress those duplicate
        ///  notifications.
        ///
        /// </remarks>
        public void SetMessageCreatedCallback(MessageCreatedCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.MessageCreatedCallback __cbDelegate =
                    NativeMethods.Client.MessageCreatedCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetMessageCreatedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sets a callback to be invoked whenever a message is deleted.
        /// </summary>
        /// <remarks>
        ///  Some messages sent from in game, as well as all messages sent from a connected user's
        ///  Discord client can be edited and deleted in the Discord client. So it is valuable to
        ///  implement support for this callback so that if a user edits or deletes a message in the
        ///  Discord client, it is reflected in game as well.
        ///
        /// </remarks>
        public void SetMessageDeletedCallback(MessageDeletedCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.MessageDeletedCallback __cbDelegate =
                    NativeMethods.Client.MessageDeletedCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetMessageDeletedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sets a callback to be invoked whenever a message is edited.
        /// </summary>
        /// <remarks>
        ///  Some messages sent from in game, as well as all messages sent from a connected user's
        ///  Discord client can be edited and deleted in the Discord client. So it is valuable to
        ///  implement support for this callback so that if a user edits or deletes a message in the
        ///  Discord client, it is reflected in game as well.
        ///
        /// </remarks>
        public void SetMessageUpdatedCallback(MessageUpdatedCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.MessageUpdatedCallback __cbDelegate =
                    NativeMethods.Client.MessageUpdatedCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetMessageUpdatedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sets whether chat messages are currently being shown in the game.
        /// </summary>
        /// <remarks>
        ///  If the user has the Discord desktop application open on the same machine as the game, then
        ///  they will hear notifications from the Discord application, even though they are able to see
        ///  those messages in game. So to avoid double-notifying users, you can call this function
        ///  whenever the chat is shown or hidden to suppress those duplicate notifications.
        ///
        ///  Keep in mind, if the game stops showing chat for a period of time, or the game loses focus
        ///  because the user switches to a different app, it is important to call this function again
        ///  so that the user's notifications get re-enabled in Discord during this time.
        ///
        /// </remarks>
        public void SetShowingChat(bool showingChat)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetShowingChat(self, showingChat);
                }
            }
        }

        /// <summary>
        ///  Adds a callback function to be invoked for each new log message generated by the SDK.
        /// </summary>
        /// <remarks>
        ///  This function explicitly excludes most logs for voice and webrtc activity since those are
        ///  generally much noisier and you may want to pick a different log level for those. So it will
        ///  instead include logs for things such as lobbies, relationships, presence, and
        ///  authentication.
        ///
        ///  We strongly recommend invoking this function immediately after constructing the Client
        ///  object.
        ///
        /// </remarks>
        public void AddLogCallback(LogCallback callback,
                                   LoggingSeverity minSeverity)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.LogCallback __callbackDelegate =
                    NativeMethods.Client.LogCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.AddLogCallback(
                        self,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback),
                        minSeverity);
                }
            }
        }

        /// <summary>
        ///  Adds a callback function to be invoked for each new log message generated by the voice
        ///  subsystem of the SDK, including the underlying webrtc infrastructure.
        /// </summary>
        /// <remarks>
        ///  We strongly recommend invoking this function immediately after constructing the Client
        ///  object.
        ///
        /// </remarks>
        public void AddVoiceLogCallback(LogCallback callback,
                                        LoggingSeverity minSeverity)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.LogCallback __callbackDelegate =
                    NativeMethods.Client.LogCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.AddVoiceLogCallback(
                        self,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback),
                        minSeverity);
                }
            }
        }

        /// <summary>
        ///  Asynchronously connects the client to Discord.
        /// </summary>
        /// <remarks>
        ///  If a client is disconnecting, this will wait for the disconnect before reconnecting.
        ///  You should use the Client::SetStatusChangedCallback and Client::GetStatus functions to
        ///  receive updates on the client status. The Client is only safe to use once the status
        ///  changes to Status::Ready.
        ///
        /// </remarks>
        public void Connect()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.Connect(self);
                }
            }
        }

        /// <summary>
        ///  Asynchronously disconnects the client.
        /// </summary>
        /// <remarks>
        ///  You can leverage Client::SetStatusChangedCallback and Client::GetStatus to receive updates
        ///  on the client status. It is fully disconnected when the status changes to
        ///  Client::Status::Disconnected.
        ///
        /// </remarks>
        public void Disconnect()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.Disconnect(self);
                }
            }
        }

        /// <summary>
        ///  Returns the current status of the client, see the Status enum for an explanation of the
        ///  possible values.
        /// </summary>
        public Status GetStatus()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Status __returnValue;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnValue = NativeMethods.Client.GetStatus(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Opens the Connected Games settings in the Discord client, which is where
        ///  users can manage their settings related to games using the Discord Social SDK.
        /// </summary>
        /// <remarks>
        ///  If the client isn't connected or the user is a provisional account, this function does
        ///  nothing.
        ///
        ///  It is always a no-op for console platforms.
        ///
        /// </remarks>
        public void OpenConnectedGamesSettingsInDiscord(
            OpenConnectedGamesSettingsInDiscordCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client
                    .OpenConnectedGamesSettingsInDiscordCallback __callbackDelegate =
                        NativeMethods.Client.OpenConnectedGamesSettingsInDiscordCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.OpenConnectedGamesSettingsInDiscord(
                        self,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }

        /// <summary>
        ///  This function is used to set the application ID for the client. This is used to
        ///  identify the application to the Discord client. This is used for things like
        ///  authentication, rich presence, and activity invites when *not* connected with
        ///  Client::Connect. When calling Client::Connect, the application ID is set automatically
        /// </summary>
        public void SetApplicationId(ulong applicationId)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetApplicationId(self, applicationId);
                }
            }
        }

        /// <summary>
        ///  Causes logs generated by the SDK to be written to disk in the specified directory.
        /// </summary>
        /// <remarks>
        ///  This function explicitly excludes most logs for voice and webrtc activity since those are
        ///  generally much noisier and you may want to pick a different log level for those. So it will
        ///  instead include logs for things such as lobbies, relationships, presence, and
        ///  authentication. An empty path defaults to logging alongside the client library. A
        ///  minSeverity = LoggingSeverity::None disables logging to a file (also the current default).
        ///  The logs will be placed into a file called "discord.log" in the specified directory.
        ///  Overwrites any existing discord.log file.
        ///
        ///  We strongly recommend invoking this function immediately after constructing the Client
        ///  object.
        ///
        ///  Returns true if the log file was successfully opened, false otherwise.
        ///
        /// </remarks>
        public bool SetLogDir(string path, LoggingSeverity minSeverity)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnValue;
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __pathSpan;
                var __pathOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__pathSpan, path);

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnValue = NativeMethods.Client.SetLogDir(self, __pathSpan, minSeverity);
                }

                NativeMethods.__FreeLocalString(&__pathSpan, __pathOwned);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Sets a callback function to be invoked whenever the SDKs status changes.
        /// </summary>
        public void SetStatusChangedCallback(OnStatusChanged cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.OnStatusChanged __cbDelegate =
                    NativeMethods.Client.OnStatusChanged_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetStatusChangedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Causes logs generated by the voice subsystem of the SDK to be written to disk in the
        ///  specified directory.
        /// </summary>
        /// <remarks>
        ///  These logs will be in a file like discord-webrtc_0, and if they grow to big will be rotated
        ///  and the number incremented. If the log files already exist the old ones will be renamed to
        ///  discord-last-webrtc_0.
        ///
        ///  An empty path defaults to logging alongside the client library.
        ///  A minSeverity = LoggingSeverity::None disables logging to a file (also the current
        ///  default).
        ///
        ///  WARNING: This function MUST be invoked immediately after constructing the Client object!
        ///  It will print out a warning if invoked too late.
        ///
        /// </remarks>
        public void SetVoiceLogDir(string path, LoggingSeverity minSeverity)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __pathSpan;
                var __pathOwned =
                    NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__pathSpan, path);

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetVoiceLogDir(self, __pathSpan, minSeverity);
                }

                NativeMethods.__FreeLocalString(&__pathSpan, __pathOwned);
            }
        }

        /// <summary>
        ///  Joins the user to the specified lobby, creating one if it does not exist.
        /// </summary>
        /// <remarks>
        ///  The lobby is specified by the supplied string, which should be a hard to guess secret
        ///  generated by the game. All users who join the lobby with the same secret will be placed in
        ///  the same lobby.
        ///
        ///  For exchanging the secret, we strongly encourage looking into the activity invite and rich
        ///  presence systems which provide a way to include a secret string that only accepted party
        ///  members are able to see.
        ///
        ///  As with server created lobbies, client created lobbies auto-delete once they have been idle
        ///  for a few minutes (which currently defaults to 5 minutes). A lobby is idle if no users are
        ///  connected to it.
        ///
        ///  This function shouldn't be used for long lived lobbies. The "secret" value expires after
        ///  ~30 days, at which point the existing lobby cannot be joined and a new one would be created
        ///  instead.
        ///
        /// </remarks>
        public void CreateOrJoinLobby(string secret,
                                      CreateOrJoinLobbyCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __secretSpan;
                var __secretOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__secretSpan, secret);
                Discord.Sdk.NativeMethods.Client.CreateOrJoinLobbyCallback __callbackDelegate =
                    NativeMethods.Client.CreateOrJoinLobbyCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.CreateOrJoinLobby(
                        self,
                        __secretSpan,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }

                NativeMethods.__FreeLocalString(&__secretSpan, __secretOwned);
            }
        }

        /// <summary>
        ///  Variant of Client::CreateOrJoinLobby that also accepts developer-supplied metadata.
        /// </summary>
        /// <remarks>
        ///  Metadata is just simple string key/value pairs.
        ///  An example use case for this might be to the internal game ID of the user of each lobby so
        ///  all members of the lobby can have a mapping of discord IDs to game IDs. Subsequent calls to
        ///  CreateOrJoinLobby will overwrite the metadata for the lobby and member.
        ///
        /// </remarks>
        public void CreateOrJoinLobbyWithMetadata(
            string secret,
            Dictionary<string, string> lobbyMetadata,
            Dictionary<string, string> memberMetadata,
            CreateOrJoinLobbyCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __secretSpan;
                var __secretOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__secretSpan, secret);
                NativeMethods.Discord_Properties __lobbyMetadataNative;
                __lobbyMetadataNative.size = (IntPtr)lobbyMetadata.Count;
                NativeMethods.Discord_String* __lobbyMetadataKeys;
                NativeMethods.Discord_String* __lobbyMetadataValues;
                bool* __lobbyMetadataKeyOwnership;
                bool* __lobbyMetadataValueOwnership;
                var __lobbyMetadataKeysOwned = NativeMethods.__AllocLocalStringArray(
                    __scratch, &__scratchUsed, 1024, &__lobbyMetadataKeys, lobbyMetadata.Count);
                var __lobbyMetadataValuesOwned = NativeMethods.__AllocLocalStringArray(
                    __scratch, &__scratchUsed, 1024, &__lobbyMetadataValues, lobbyMetadata.Count);
                var __lobbyMetadataKeyOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
                    __scratch, &__scratchUsed, 1024, &__lobbyMetadataKeyOwnership, lobbyMetadata.Count);
                var __lobbyMetadataValueOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
                    __scratch, &__scratchUsed, 1024, &__lobbyMetadataValueOwnership, lobbyMetadata.Count);
                {
                    int __i = 0;

                    foreach (var (__lobbyMetadataKey, __lobbyMetadataValue) in lobbyMetadata)
                    {
                        NativeMethods.Discord_String __lobbyMetadataKeySpan;
                        NativeMethods.Discord_String __lobbyMetadataValueSpan;
                        __lobbyMetadataKeyOwnership[__i] = NativeMethods.__InitStringLocal(
                            __scratch, &__scratchUsed, 1024, &__lobbyMetadataKeySpan, __lobbyMetadataKey);
                        __lobbyMetadataValueOwnership[__i] =
                            NativeMethods.__InitStringLocal(__scratch,
                                &__scratchUsed,
                                1024,
                                &__lobbyMetadataValueSpan,
                                __lobbyMetadataValue);
                        __lobbyMetadataKeys[__i] = __lobbyMetadataKeySpan;
                        __lobbyMetadataValues[__i] = __lobbyMetadataValueSpan;
                        __i++;
                    }
                }
                __lobbyMetadataNative.keys = __lobbyMetadataKeys;
                __lobbyMetadataNative.values = __lobbyMetadataValues;
                NativeMethods.Discord_Properties __memberMetadataNative;
                __memberMetadataNative.size = (IntPtr)memberMetadata.Count;
                NativeMethods.Discord_String* __memberMetadataKeys;
                NativeMethods.Discord_String* __memberMetadataValues;
                bool* __memberMetadataKeyOwnership;
                bool* __memberMetadataValueOwnership;
                var __memberMetadataKeysOwned = NativeMethods.__AllocLocalStringArray(
                    __scratch, &__scratchUsed, 1024, &__memberMetadataKeys, memberMetadata.Count);
                var __memberMetadataValuesOwned = NativeMethods.__AllocLocalStringArray(
                    __scratch, &__scratchUsed, 1024, &__memberMetadataValues, memberMetadata.Count);
                var __memberMetadataKeyOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
                    __scratch, &__scratchUsed, 1024, &__memberMetadataKeyOwnership, memberMetadata.Count);
                var __memberMetadataValueOwnershipOwned =
                    NativeMethods.__AllocateLocalBoolArray(__scratch,
                        &__scratchUsed,
                        1024,
                        &__memberMetadataValueOwnership,
                        memberMetadata.Count);
                {
                    int __i = 0;

                    foreach (var (__memberMetadataKey, __memberMetadataValue) in memberMetadata)
                    {
                        NativeMethods.Discord_String __memberMetadataKeySpan;
                        NativeMethods.Discord_String __memberMetadataValueSpan;
                        __memberMetadataKeyOwnership[__i] =
                            NativeMethods.__InitStringLocal(__scratch,
                                &__scratchUsed,
                                1024,
                                &__memberMetadataKeySpan,
                                __memberMetadataKey);
                        __memberMetadataValueOwnership[__i] =
                            NativeMethods.__InitStringLocal(__scratch,
                                &__scratchUsed,
                                1024,
                                &__memberMetadataValueSpan,
                                __memberMetadataValue);
                        __memberMetadataKeys[__i] = __memberMetadataKeySpan;
                        __memberMetadataValues[__i] = __memberMetadataValueSpan;
                        __i++;
                    }
                }
                __memberMetadataNative.keys = __memberMetadataKeys;
                __memberMetadataNative.values = __memberMetadataValues;
                Discord.Sdk.NativeMethods.Client.CreateOrJoinLobbyCallback __callbackDelegate =
                    NativeMethods.Client.CreateOrJoinLobbyCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.CreateOrJoinLobbyWithMetadata(
                        self,
                        __secretSpan,
                        __lobbyMetadataNative,
                        __memberMetadataNative,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }

                for (int __i = 0; __i < (int)__memberMetadataNative.size; __i++)
                {
                    NativeMethods.__FreeLocalString(&__memberMetadataKeys[__i],
                        __memberMetadataKeyOwnership[__i]);
                    NativeMethods.__FreeLocalString(&__memberMetadataValues[__i],
                        __memberMetadataValueOwnership[__i]);
                }

                NativeMethods.__FreeLocal(__memberMetadataKeys, __memberMetadataKeysOwned);
                NativeMethods.__FreeLocal(__memberMetadataValues, __memberMetadataValuesOwned);

                for (int __i = 0; __i < (int)__lobbyMetadataNative.size; __i++)
                {
                    NativeMethods.__FreeLocalString(&__lobbyMetadataKeys[__i],
                        __lobbyMetadataKeyOwnership[__i]);
                    NativeMethods.__FreeLocalString(&__lobbyMetadataValues[__i],
                        __lobbyMetadataValueOwnership[__i]);
                }

                NativeMethods.__FreeLocal(__lobbyMetadataKeys, __lobbyMetadataKeysOwned);
                NativeMethods.__FreeLocal(__lobbyMetadataValues, __lobbyMetadataValuesOwned);
                NativeMethods.__FreeLocalString(&__secretSpan, __secretOwned);
            }
        }

        /// <summary>
        ///  Fetches all of the channels that the current user can access in the given guild.
        ///  Channels are sorted by their `position` field, which matches what you see in the Discord
        ///  client.
        /// </summary>
        /// <remarks>
        ///  The purpose of this is to power the channel linking flow for linking a Discord channel to
        ///  an in-game lobby. So this function can be used to power a UI to let the user pick which
        ///  channel to link to once they have picked a guild. See the docs on LobbyHandle for more
        ///  information.
        ///
        /// </remarks>
        public void GetGuildChannels(ulong guildId, GetGuildChannelsCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.GetGuildChannelsCallback __cbDelegate =
                    NativeMethods.Client.GetGuildChannelsCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetGuildChannels(
                        self,
                        guildId,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Returns a reference to the Discord lobby object for the given ID.
        /// </summary>
        public LobbyHandle? GetLobbyHandle(ulong lobbyId)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.LobbyHandle();
                LobbyHandle? __returnValue = null;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.Client.GetLobbyHandle(self, lobbyId, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new LobbyHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns a list of all the lobbies that the user is a member of and the SDK has loaded.
        /// </summary>
        /// <remarks>
        ///  Lobbies are optimistically loaded when the SDK starts but in some cases may not be
        ///  available immediately after the SDK status changes to Status::Ready.
        ///
        /// </remarks>
        public ulong[] GetLobbyIds()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_UInt64Span();

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetLobbyIds(self, &__returnValue);
                }

                var __returnValueSurface =
                    new Span<ulong>(__returnValue.ptr, (int)__returnValue.size).ToArray();
                NativeMethods.Discord_Free(__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Fetches all of the guilds (also known as Discord servers) that the current user is a member
        ///  of.
        /// </summary>
        /// <remarks>
        ///  The purpose of this is to power the channel linking flow for linking a Discord channel
        ///  to an in-game lobby. So this function can be used to power a UI to let the user which guild
        ///  to link to. See the docs on LobbyHandle for more information.
        ///
        /// </remarks>
        public void GetUserGuilds(GetUserGuildsCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.GetUserGuildsCallback __cbDelegate =
                    NativeMethods.Client.GetUserGuildsCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetUserGuilds(self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Invites the current user to the Discord guild of the channel that is linked to the
        ///  specified lobby. The user is forwarded to the Discord client with the resulting invite url,
        ///  upon which the user can decide to accept or decline that invite.
        /// </summary>
        /// <remarks>
        ///  On console platforms, the user is not navigated to any Discord client, so the invite url
        ///  should be presented to the user in some way, so they can use it.
        ///
        /// </remarks>
        public void JoinLinkedLobbyGuild(
            ulong lobbyId,
            ProvisionalUserMergeRequiredCallback provisionalUserMergeRequiredCallback,
            JoinLinkedLobbyGuildCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client
                    .ProvisionalUserMergeRequiredCallback __provisionalUserMergeRequiredCallbackDelegate =
                        NativeMethods.Client.ProvisionalUserMergeRequiredCallback_Handler;
                Discord.Sdk.NativeMethods.Client.JoinLinkedLobbyGuildCallback __callbackDelegate =
                    NativeMethods.Client.JoinLinkedLobbyGuildCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.JoinLinkedLobbyGuild(
                        self,
                        lobbyId,
                        __provisionalUserMergeRequiredCallbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(provisionalUserMergeRequiredCallback),
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }

        /// <summary>
        ///  Removes the current user from the specified lobby.
        /// </summary>
        /// <remarks>
        ///  Only lobbies that contain a "secret" can be left.
        ///  In other words, only lobbies created with Client::CreateOrJoinLobby can be left.
        ///  Lobbies created using the server API may not be manipulated by clients, so you must
        ///  use the server API to remove them too.
        ///
        /// </remarks>
        public void LeaveLobby(ulong lobbyId, LeaveLobbyCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.LeaveLobbyCallback __callbackDelegate =
                    NativeMethods.Client.LeaveLobbyCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.LeaveLobby(
                        self,
                        lobbyId,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }

        /// <summary>
        ///  Links the specified channel on Discord to the specified in-game lobby.
        /// </summary>
        /// <remarks>
        ///  Any message sent in one will be copied over to the other!
        ///  See the docs on LobbyHandle for more information.
        ///
        /// </remarks>
        public void LinkChannelToLobby(ulong lobbyId,
                                       ulong channelId,
                                       LinkOrUnlinkChannelCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.LinkOrUnlinkChannelCallback __callbackDelegate =
                    NativeMethods.Client.LinkOrUnlinkChannelCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.LinkChannelToLobby(
                        self,
                        lobbyId,
                        channelId,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }

        /// <summary>
        ///  Sets a callback to be invoked when a lobby "becomes available" to the client.
        /// </summary>
        /// <remarks>
        ///  A lobby can become available in a few situations:
        ///  - A new lobby is created and the current user is a member of it
        ///  - The current user is added to an existing lobby
        ///  - A lobby recovers after a backend crash and is available once again
        ///
        ///  This means that the LobbyCreated callback can be invoked more than once in a single
        ///  session! Generally though it should never be invoked twice in a row. For example if a lobby
        ///  crashes or a user is removed from the lobby, you should expect to have the LobbyDeleted
        ///  callback invoked first.
        ///
        /// </remarks>
        public void SetLobbyCreatedCallback(LobbyCreatedCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.LobbyCreatedCallback __cbDelegate =
                    NativeMethods.Client.LobbyCreatedCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetLobbyCreatedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sets a callback to be invoked when a lobby is no longer available.
        /// </summary>
        /// <remarks>
        ///  This callback can be invoked in a number of situations:
        ///  - A lobby the user is a member of is deleted
        ///  - The current user is removed from a lobby
        ///  - There is a backend crash that causes the lobby to be unavailable for all users
        ///
        ///  This means that this callback might be invoked even though the lobby still exists for other
        ///  users!
        ///
        /// </remarks>
        public void SetLobbyDeletedCallback(LobbyDeletedCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.LobbyDeletedCallback __cbDelegate =
                    NativeMethods.Client.LobbyDeletedCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetLobbyDeletedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sets a callback function to be invoked whenever a user is added to a lobby.
        /// </summary>
        /// <remarks>
        ///  This callback will not be invoked when the current user is added to a lobby, instead the
        ///  LobbyCreated callback will be invoked. Additionally, the SDK separates membership in a
        ///  lobby from whether a user is connected to a lobby. So a user being added does not
        ///  necessarily mean they are online and in the lobby at that moment, just that they have
        ///  permission to connect to that lobby.
        ///
        /// </remarks>
        public void SetLobbyMemberAddedCallback(LobbyMemberAddedCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.LobbyMemberAddedCallback __cbDelegate =
                    NativeMethods.Client.LobbyMemberAddedCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetLobbyMemberAddedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sets a callback function to be invoked whenever a member of a lobby is removed and can no
        ///  longer connect to it.
        /// </summary>
        /// <remarks>
        ///  This callback will not be invoked when the current user is removed from a lobby, instead
        ///  LobbyDeleted callback will be invoked. Additionally this is not invoked when a user simply
        ///  exits the game. That would cause the LobbyMemberUpdatedCallback to be invoked, and the
        ///  LobbyMemberHandle object will indicate they are not connected now.
        ///
        /// </remarks>
        public void SetLobbyMemberRemovedCallback(LobbyMemberRemovedCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.LobbyMemberRemovedCallback __cbDelegate =
                    NativeMethods.Client.LobbyMemberRemovedCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetLobbyMemberRemovedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sets a callback function to be invoked whenever a member of a lobby is changed.
        /// </summary>
        /// <remarks>
        ///  This is invoked when:
        ///  - The user connects or disconnects
        ///  - The metadata of the member is changed
        ///
        /// </remarks>
        public void SetLobbyMemberUpdatedCallback(LobbyMemberUpdatedCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.LobbyMemberUpdatedCallback __cbDelegate =
                    NativeMethods.Client.LobbyMemberUpdatedCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetLobbyMemberUpdatedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sets a callback to be invoked when a lobby is edited, for example if the lobby's metadata
        ///  is changed.
        /// </summary>
        public void SetLobbyUpdatedCallback(LobbyUpdatedCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.LobbyUpdatedCallback __cbDelegate =
                    NativeMethods.Client.LobbyUpdatedCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetLobbyUpdatedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Removes any existing channel link from the specified lobby.
        /// </summary>
        /// <remarks>
        ///  See the docs on LobbyHandle for more information.
        ///  A lobby can be unlinked by any user with the LobbyMemberFlags::CanLinkLobby flag, they do
        ///  not need to have any permissions on the Discord channel in order to sever the in-game link.
        ///
        /// </remarks>
        public void UnlinkChannelFromLobby(ulong lobbyId,
                                           LinkOrUnlinkChannelCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.LinkOrUnlinkChannelCallback __callbackDelegate =
                    NativeMethods.Client.LinkOrUnlinkChannelCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.UnlinkChannelFromLobby(
                        self,
                        lobbyId,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }

        /// <summary>
        ///  Checks whether the Discord mobile app is installed on this device.
        ///  On desktop platforms, always returns false.
        /// </summary>
        /// <remarks>
        ///  This check does not require a client connection and can be called at any time.
        ///
        ///  This can be used to provide UI hints to users about whether they can authorize via the
        ///  Discord app, or whether they will need to use a web browser flow.
        ///
        ///  Platform Requirements:
        ///  - iOS: Your app must include "discord" in the LSApplicationQueriesSchemes array
        ///    in your Info.plist for this check to work correctly.
        ///  - Android: Your app must include "com.discord" in the `queries` element
        ///    in your AndroidManifest.xml (required for Android 11+).
        ///
        /// </remarks>
        public void IsDiscordAppInstalled(IsDiscordAppInstalledCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.IsDiscordAppInstalledCallback __callbackDelegate =
                    NativeMethods.Client.IsDiscordAppInstalledCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.IsDiscordAppInstalled(
                        self,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }

        /// <summary>
        ///  Accepts an activity invite that the current user has received.
        /// </summary>
        /// <remarks>
        ///  The given callback will be invoked with the join secret for the activity, which can be used
        ///  to join the user to the game's internal party system for example.
        ///  This join secret comes from the other user's rich presence activity.
        ///
        /// </remarks>
        public void AcceptActivityInvite(ActivityInvite invite,
                                         AcceptActivityInviteCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityInvite* __inviteFixed = &invite.self)
                {
                    Discord.Sdk.NativeMethods.Client.AcceptActivityInviteCallback __cbDelegate =
                        NativeMethods.Client.AcceptActivityInviteCallback_Handler;

                    fixed (NativeMethods.Client* self = &this.self)
                    {
                        NativeMethods.Client.AcceptActivityInvite(
                            self,
                            __inviteFixed,
                            __cbDelegate,
                            NativeMethods.ManagedUserData.Free,
                            NativeMethods.ManagedUserData.CreateHandle(cb));
                    }
                }
            }
        }

        /// <summary>
        ///  Clears the right presence for the current user.
        /// </summary>
        public void ClearRichPresence()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.ClearRichPresence(self);
                }
            }
        }

        /// <summary>
        ///  When a user accepts an activity invite for your game within the Discord client, Discord
        ///  needs to know how to launch the game for that user. This function allows you to register a
        ///  command that Discord will run to launch your game. You should invoke this when the SDK
        ///  starts up so that if the user in the future tries to join from Discord the game will be
        ///  able to be launched for them. Returns true if the command was successfully registered,
        ///  false otherwise.
        /// </summary>
        /// <remarks>
        ///  On Windows and Linux, this command should be a path to an executable. It also supports any
        ///  launch parameters that may be needed, like
        ///  "C:\path\to my\game.exe" --full-screen --no-hax
        ///  If you pass an empty string in for the command, the SDK will register the current running
        ///  executable. To launch the game from a custom protocol like my-awesome-game://, pass that in
        ///  as an argument of the executable that should be launched by that protocol. For example,
        ///  "C:\path\to my\game.exe" my-awesome-game://
        ///
        ///  On macOS, due to the way Discord registers executables, your game needs to be bundled for
        ///  this command to work. That means it should be a .app. You can pass a custom protocol like
        ///  my-awesome-game:// as the custom command, but *not* a path to an executable. If you pass an
        ///  empty string in for the command, the SDK will register the current running bundle, if any.
        ///
        /// </remarks>
        public bool RegisterLaunchCommand(ulong applicationId, string command)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnValue;
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __commandSpan;
                var __commandOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__commandSpan, command);

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnValue =
                        NativeMethods.Client.RegisterLaunchCommand(self, applicationId, __commandSpan);
                }

                NativeMethods.__FreeLocalString(&__commandSpan, __commandOwned);
                return __returnValue;
            }
        }

        /// <summary>
        ///  When a user accepts an activity invite for your game within the Discord client, Discord
        ///  needs to know how to launch the game for that user. For steam games, this function allows
        ///  you to indicate to Discord what the steam game ID is. You should invoke this when the SDK
        ///  starts up so that if the user in the future tries to join from Discord the game will be
        ///  able to be launched for them. Returns true if the command was successfully registered,
        ///  false otherwise.
        /// </summary>
        public bool RegisterLaunchSteamApplication(ulong applicationId, uint steamAppId)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnValue;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnValue = NativeMethods.Client.RegisterLaunchSteamApplication(
                        self, applicationId, steamAppId);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Sends a Discord activity invite to the specified user.
        /// </summary>
        /// <remarks>
        ///  The invite is sent as a message on Discord, which means it can be sent if any
        ///  of the following are true:
        ///  - Both users are online and in the game and have not blocked each other
        ///  - Both users are friends with each other
        ///  - Both users share a mutual Discord server and have previously DM'd each other on Discord
        ///
        ///  You can optionally include some message content to include in the message containing the
        ///  invite, but it's ok to pass an empty string too.
        ///
        /// </remarks>
        public void SendActivityInvite(ulong userId,
                                       string content,
                                       SendActivityInviteCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __contentSpan;
                var __contentOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__contentSpan, content);
                Discord.Sdk.NativeMethods.Client.SendActivityInviteCallback __cbDelegate =
                    NativeMethods.Client.SendActivityInviteCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SendActivityInvite(
                        self,
                        userId,
                        __contentSpan,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }

                NativeMethods.__FreeLocalString(&__contentSpan, __contentOwned);
            }
        }

        /// <summary>
        ///  Requests to join the activity of the specified user.
        /// </summary>
        /// <remarks>
        ///  This can be called whenever the target user has a rich presence activity for the current
        ///  game and that activity has space for another user to join them.
        ///
        ///  That user will basically receive an activity invite which they can accept or reject.
        ///
        /// </remarks>
        public void SendActivityJoinRequest(ulong userId,
                                            SendActivityInviteCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.SendActivityInviteCallback __cbDelegate =
                    NativeMethods.Client.SendActivityInviteCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SendActivityJoinRequest(
                        self,
                        userId,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  When another user requests to join the current user's party, this function is called to
        ///  to allow that user to join. Specifically this will send the original user an activity
        ///  invite which they then need to accept again.
        /// </summary>
        public void SendActivityJoinRequestReply(ActivityInvite invite,
                                                 SendActivityInviteCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.ActivityInvite* __inviteFixed = &invite.self)
                {
                    Discord.Sdk.NativeMethods.Client.SendActivityInviteCallback __cbDelegate =
                        NativeMethods.Client.SendActivityInviteCallback_Handler;

                    fixed (NativeMethods.Client* self = &this.self)
                    {
                        NativeMethods.Client.SendActivityJoinRequestReply(
                            self,
                            __inviteFixed,
                            __cbDelegate,
                            NativeMethods.ManagedUserData.Free,
                            NativeMethods.ManagedUserData.CreateHandle(cb));
                    }
                }
            }
        }

        /// <summary>
        ///  Sets a callback function that is invoked when the current user receives an activity invite
        ///  from another user.
        /// </summary>
        /// <remarks>
        ///  These invites are always sent as messages, so the SDK is parsing these
        ///  messages to look for invites and invokes this callback instead. The message create callback
        ///  will not be invoked for these messages. The invite object contains all the necessary
        ///  information to identity the invite, which you can later pass to
        ///  Client::AcceptActivityInvite.
        ///
        /// </remarks>
        public void SetActivityInviteCreatedCallback(ActivityInviteCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.ActivityInviteCallback __cbDelegate =
                    NativeMethods.Client.ActivityInviteCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetActivityInviteCreatedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sets a callback function that is invoked when an existing activity invite changes.
        ///  Currently, the only thing that changes on an activity invite is its validity. If the sender
        ///  goes offline or exits the party the receiver was invited to, the invite is no longer
        ///  joinable. It is possible for an invalid invite to go from invalid to valid if the sender
        ///  rejoins the activity.
        /// </summary>
        public void SetActivityInviteUpdatedCallback(ActivityInviteCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.ActivityInviteCallback __cbDelegate =
                    NativeMethods.Client.ActivityInviteCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetActivityInviteUpdatedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sets a callback function that is invoked when the current user also has Discord running on
        ///  their computer and they accept an activity invite in the Discord client.
        /// </summary>
        /// <remarks>
        ///  This callback is invoked with the join secret from the activity rich presence, which you
        ///  can use to join them to the game's internal party system. See Activity for more information
        ///  on invites.
        ///
        /// </remarks>
        public void SetActivityJoinCallback(ActivityJoinCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.ActivityJoinCallback __cbDelegate =
                    NativeMethods.Client.ActivityJoinCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetActivityJoinCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sets a callback function that is invoked when the current user also has Discord running on
        ///  their computer and they accept an activity invite in the Discord client.
        /// </summary>
        /// <remarks>
        ///  This callback is invoked with the join secret from the activity rich presence, which you
        ///  can use to join them to the game's internal party system. See Activity for more information
        ///  on invites.
        ///
        /// </remarks>
        public void SetActivityJoinWithApplicationCallback(
            ActivityJoinWithApplicationCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.ActivityJoinWithApplicationCallback __cbDelegate =
                    NativeMethods.Client.ActivityJoinWithApplicationCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetActivityJoinWithApplicationCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sets whether a user is online/invisible/idle/dnd on Discord.
        /// </summary>
        public void SetOnlineStatus(StatusType status,
                                    UpdateStatusCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.UpdateStatusCallback __callbackDelegate =
                    NativeMethods.Client.UpdateStatusCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetOnlineStatus(
                        self,
                        status,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }

        /// <summary>
        ///  Updates the rich presence for the current user.
        /// </summary>
        /// <remarks>
        ///  You should use rich presence so that other users on Discord know this user is playing a
        ///  game and you can include some hints of what they are playing such as a character name or
        ///  map name. Rich presence also enables Discord game invites to work too!
        ///
        ///  Note: On Desktop, rich presence can be set before calling Client::Connect, but it will be
        ///  cleared if the Client connects. When Client is not connected, this sets the rich presence
        ///  in the current user's Discord client when available.
        ///
        ///  See the docs on the Activity struct for more details.
        ///
        ///  Note: The Activity object here is a partial object, fields such as name, and applicationId
        ///  cannot be set and will be overwritten by the SDK. See
        ///  https://discord.com/developers/docs/rich-presence/using-with-the-game-sdk#partial-activity-struct
        ///  for more information.
        ///
        /// </remarks>
        public void UpdateRichPresence(Activity activity,
                                       UpdateRichPresenceCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                fixed (NativeMethods.Activity* __activityFixed = &activity.self)
                {
                    Discord.Sdk.NativeMethods.Client.UpdateRichPresenceCallback __cbDelegate =
                        NativeMethods.Client.UpdateRichPresenceCallback_Handler;

                    fixed (NativeMethods.Client* self = &this.self)
                    {
                        NativeMethods.Client.UpdateRichPresence(
                            self,
                            __activityFixed,
                            __cbDelegate,
                            NativeMethods.ManagedUserData.Free,
                            NativeMethods.ManagedUserData.CreateHandle(cb));
                    }
                }
            }
        }

        /// <summary>
        ///  Accepts an incoming Discord friend request from the target user.
        /// </summary>
        /// <remarks>
        ///  Fails if the target user has not sent a Discord friend request to the current user, meaning
        ///  that the Discord relationship type between the users must be
        ///  RelationshipType::PendingIncoming.
        ///
        /// </remarks>
        public void AcceptDiscordFriendRequest(ulong userId,
                                               UpdateRelationshipCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
                    NativeMethods.Client.UpdateRelationshipCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.AcceptDiscordFriendRequest(
                        self,
                        userId,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Accepts an incoming game friend request from the target user.
        /// </summary>
        /// <remarks>
        ///  Fails if the target user has not sent a game friend request to the current user, meaning
        ///  that the game relationship type between the users must be
        ///  RelationshipType::PendingIncoming.
        ///
        /// </remarks>
        public void AcceptGameFriendRequest(ulong userId,
                                            UpdateRelationshipCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
                    NativeMethods.Client.UpdateRelationshipCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.AcceptGameFriendRequest(
                        self,
                        userId,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Blocks the target user so that they cannot send the user friend or activity invites and
        ///  cannot message them anymore.
        /// </summary>
        /// <remarks>
        ///  Blocking a user will also remove any existing relationship
        ///  between the two users, and persists across games, so blocking a user in one game or on
        ///  Discord will block them in all other games and on Discord as well.
        ///
        /// </remarks>
        public void BlockUser(ulong userId, UpdateRelationshipCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
                    NativeMethods.Client.UpdateRelationshipCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.BlockUser(self,
                        userId,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Cancels an outgoing Discord friend request to the target user.
        /// </summary>
        /// <remarks>
        ///  Fails if a Discord friend request has not been sent to the target user, meaning
        ///  that the Discord relationship type between the users must be
        ///  RelationshipType::PendingOutgoing.
        ///
        /// </remarks>
        public void CancelDiscordFriendRequest(ulong userId,
                                               UpdateRelationshipCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
                    NativeMethods.Client.UpdateRelationshipCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.CancelDiscordFriendRequest(
                        self,
                        userId,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Cancels an outgoing game friend request to the target user.
        /// </summary>
        /// <remarks>
        ///  Fails if a game friend request has not been sent to the target user, meaning
        ///  that the game relationship type between the users must be
        ///  RelationshipType::PendingOutgoing.
        ///
        /// </remarks>
        public void CancelGameFriendRequest(ulong userId,
                                            UpdateRelationshipCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
                    NativeMethods.Client.UpdateRelationshipCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.CancelGameFriendRequest(
                        self,
                        userId,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Returns the RelationshipHandle that corresponds to the relationship between the current
        ///  user and the given user.
        /// </summary>
        public RelationshipHandle GetRelationshipHandle(ulong userId)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __returnValueNative = new NativeMethods.RelationshipHandle();
                RelationshipHandle? __returnValue = null;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetRelationshipHandle(self, userId, &__returnValueNative);
                }

                __returnValue = new RelationshipHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns a list of all of the relationships the current user has with others, including all
        ///  Discord relationships and all Game relationships for the current game.
        /// </summary>
        public RelationshipHandle[] GetRelationships()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_RelationshipHandleSpan();

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetRelationships(self, &__returnValue);
                }

                var __returnValueSurface = new RelationshipHandle[(int)__returnValue.size];

                for (int __i = 0; __i < (int)__returnValue.size; __i++)
                {
                    __returnValueSurface[__i] = new RelationshipHandle(__returnValue.ptr[__i], 0);
                }

                NativeMethods.Discord_Free(__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Returns a list of relationships that belong to the specified relationship group type.
        ///  Relationships are logically partitioned into groups based on online status and game
        ///  activity:
        ///  - OnlinePlayingGame: Users who are online and currently playing the game
        ///  - OnlineElsewhere: Users who are online but not playing the game (users who have played the
        ///  game before are sorted to the top)
        ///  - Offline: Users who are offline
        /// </summary>
        public RelationshipHandle[] GetRelationshipsByGroup(
            RelationshipGroupType groupType)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_RelationshipHandleSpan();

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetRelationshipsByGroup(self, groupType, &__returnValue);
                }

                var __returnValueSurface = new RelationshipHandle[(int)__returnValue.size];

                for (int __i = 0; __i < (int)__returnValue.size; __i++)
                {
                    __returnValueSurface[__i] = new RelationshipHandle(__returnValue.ptr[__i], 0);
                }

                NativeMethods.Discord_Free(__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Declines an incoming Discord friend request from the target user.
        /// </summary>
        /// <remarks>
        ///  Fails if the target user has not sent a Discord friend request to the current user, meaning
        ///  that the Discord relationship type between the users must be
        ///  RelationshipType::PendingIncoming.
        ///
        /// </remarks>
        public void RejectDiscordFriendRequest(ulong userId,
                                               UpdateRelationshipCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
                    NativeMethods.Client.UpdateRelationshipCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.RejectDiscordFriendRequest(
                        self,
                        userId,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Declines an incoming game friend request from the target user.
        /// </summary>
        /// <remarks>
        ///  Fails if the target user has not sent a game friend request to the current user, meaning
        ///  that the game relationship type between the users must be
        ///  RelationshipType::PendingIncoming.
        ///
        /// </remarks>
        public void RejectGameFriendRequest(ulong userId,
                                            UpdateRelationshipCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
                    NativeMethods.Client.UpdateRelationshipCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.RejectGameFriendRequest(
                        self,
                        userId,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Removes any friendship between the current user and the target user. This function will
        ///  remove BOTH any Discord friendship and any game friendship between the users.
        /// </summary>
        /// <remarks>
        ///  Fails if the target user is not currently a Discord OR game friend with the current user.
        ///
        /// </remarks>
        public void RemoveDiscordAndGameFriend(ulong userId,
                                               UpdateRelationshipCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
                    NativeMethods.Client.UpdateRelationshipCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.RemoveDiscordAndGameFriend(
                        self,
                        userId,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Removes any game friendship between the current user and the target user.
        /// </summary>
        /// <remarks>
        ///  Fails if the target user is not currently a game friend with the current user.
        ///
        /// </remarks>
        public void RemoveGameFriend(ulong userId, UpdateRelationshipCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
                    NativeMethods.Client.UpdateRelationshipCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.RemoveGameFriend(
                        self,
                        userId,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Searches all of your friends by both username and display name, returning
        ///  a list of all friends that match the search string.
        /// </summary>
        /// <remarks>
        ///  Under the hood uses the Levenshtein distance algorithm.
        ///
        /// </remarks>
        public UserHandle[] SearchFriendsByUsername(string searchStr)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_UserHandleSpan();
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __searchStrSpan;
                var __searchStrOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__searchStrSpan, searchStr);

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SearchFriendsByUsername(self, __searchStrSpan, &__returnValue);
                }

                NativeMethods.__FreeLocalString(&__searchStrSpan, __searchStrOwned);
                var __returnValueSurface = new UserHandle[(int)__returnValue.size];

                for (int __i = 0; __i < (int)__returnValue.size; __i++)
                {
                    __returnValueSurface[__i] = new UserHandle(__returnValue.ptr[__i], 0);
                }

                NativeMethods.Discord_Free(__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Sends a Discord friend request to the target user.
        /// </summary>
        /// <remarks>
        ///  The target user is identified by their Discord unique username (not their DisplayName).
        ///
        ///  After the friend request is sent, each user will have a new Discord relationship created.
        ///  For the current user the RelationshipHandle::DiscordRelationshipType will be
        ///  RelationshipType::PendingOutgoing and for the target user it will be
        ///  RelationshipType::PendingIncoming.
        ///
        ///  If the current user already has received a Discord friend request from the target user
        ///  (meaning RelationshipHandle::DiscordRelationshipType is RelationshipType::PendingIncoming),
        ///  then the two users will become Discord friends.
        ///
        ///  See RelationshipHandle for more information on the difference between Discord and Game
        ///  relationships.
        ///
        /// </remarks>
        public void SendDiscordFriendRequest(string username,
                                             SendFriendRequestCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __usernameSpan;
                var __usernameOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__usernameSpan, username);
                Discord.Sdk.NativeMethods.Client.SendFriendRequestCallback __cbDelegate =
                    NativeMethods.Client.SendFriendRequestCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SendDiscordFriendRequest(
                        self,
                        __usernameSpan,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }

                NativeMethods.__FreeLocalString(&__usernameSpan, __usernameOwned);
            }
        }

        /// <summary>
        ///  Sends a Discord friend request to the target user.
        /// </summary>
        /// <remarks>
        ///  The target user is identified by their Discord ID.
        ///
        ///  After the friend request is sent, each user will have a new Discord relationship created.
        ///  For the current user the RelationshipHandle::DiscordRelationshipType will be
        ///  RelationshipType::PendingOutgoing and for the target user it will be
        ///  RelationshipType::PendingIncoming.
        ///
        ///  If the current user already has received a Discord friend request from the target user
        ///  (meaning RelationshipHandle::DiscordRelationshipType is RelationshipType::PendingIncoming),
        ///  then the two users will become Discord friends.
        ///
        ///  See RelationshipHandle for more information on the difference between Discord and Game
        ///  relationships.
        ///
        /// </remarks>
        public void SendDiscordFriendRequestById(ulong userId,
                                                 UpdateRelationshipCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
                    NativeMethods.Client.UpdateRelationshipCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SendDiscordFriendRequestById(
                        self,
                        userId,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sends (or accepts) a game friend request to the target user.
        /// </summary>
        /// <remarks>
        ///  The target user is identified by their Discord unique username (not their DisplayName).
        ///
        ///  After the friend request is sent, each user will have a new game relationship created. For
        ///  the current user the RelationshipHandle::GameRelationshipType will be
        ///  RelationshipType::PendingOutgoing and for the target user it will be
        ///  RelationshipType::PendingIncoming.
        ///
        ///  If the current user already has received a game friend request from the target user
        ///  (meaning RelationshipHandle::GameRelationshipType is RelationshipType::PendingIncoming),
        ///  then the two users will become game friends.
        ///
        ///  See RelationshipHandle for more information on the difference between Discord and Game
        ///  relationships.
        ///
        /// </remarks>
        public void SendGameFriendRequest(string username,
                                          SendFriendRequestCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                var __scratchAligned = stackalloc ulong[128];
                var __scratch = (byte*)__scratchAligned;
                var __scratchUsed = 0;
                NativeMethods.Discord_String __usernameSpan;
                var __usernameOwned = NativeMethods.__InitStringLocal(
                    __scratch, &__scratchUsed, 1024, &__usernameSpan, username);
                Discord.Sdk.NativeMethods.Client.SendFriendRequestCallback __cbDelegate =
                    NativeMethods.Client.SendFriendRequestCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SendGameFriendRequest(
                        self,
                        __usernameSpan,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }

                NativeMethods.__FreeLocalString(&__usernameSpan, __usernameOwned);
            }
        }

        /// <summary>
        ///  Sends (or accepts) a game friend request to the target user.
        /// </summary>
        /// <remarks>
        ///  The target user is identified by their Discord ID.
        ///
        ///  After the friend request is sent, each user will have a new game relationship created. For
        ///  the current user the RelationshipHandle::GameRelationshipType will be
        ///  RelationshipType::PendingOutgoing and for the target user it will be
        ///  RelationshipType::PendingIncoming.
        ///
        ///  If the current user already has received a game friend request from the target user
        ///  (meaning RelationshipHandle::GameRelationshipType is RelationshipType::PendingIncoming),
        ///  then the two users will become game friends.
        ///
        ///  See RelationshipHandle for more information on the difference between Discord and Game
        ///  relationships.
        ///
        /// </remarks>
        public void SendGameFriendRequestById(ulong userId,
                                              UpdateRelationshipCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
                    NativeMethods.Client.UpdateRelationshipCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SendGameFriendRequestById(
                        self,
                        userId,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sets a callback to be invoked whenever a relationship for this user is established or
        ///  changes type.
        /// </summary>
        /// <remarks>
        ///  This can be invoked when a user sends or accepts a friend invite or blocks a user for
        ///  example.
        ///
        /// </remarks>
        public void SetRelationshipCreatedCallback(RelationshipCreatedCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.RelationshipCreatedCallback __cbDelegate =
                    NativeMethods.Client.RelationshipCreatedCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetRelationshipCreatedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Sets a callback to be invoked whenever a relationship for this user is removed,
        ///  such as when the user rejects a friend request or removes a friend.
        /// </summary>
        /// <remarks>
        ///  When a relationship is removed, Client::GetRelationshipHandle will
        ///  return a relationship with the type set to RelationshipType::None.
        ///
        /// </remarks>
        public void SetRelationshipDeletedCallback(RelationshipDeletedCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.RelationshipDeletedCallback __cbDelegate =
                    NativeMethods.Client.RelationshipDeletedCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetRelationshipDeletedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Unblocks the target user. Does not restore any old relationship between the users though.
        /// </summary>
        /// <remarks>
        ///  Fails if the target user is not currently blocked.
        ///
        /// </remarks>
        public void UnblockUser(ulong userId, UpdateRelationshipCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
                    NativeMethods.Client.UpdateRelationshipCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.UnblockUser(self,
                        userId,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  Unlike GetCurrentUser(), this method returns std::nullopt instead of a dummy object
        ///  when no user is authenticated or available. This provides clearer intent about when
        ///  the user data is actually available.
        /// </summary>
        public UserHandle? GetCurrentUserV2()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.UserHandle();
                UserHandle? __returnValue = null;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.Client.GetCurrentUserV2(self, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new UserHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  If the Discord app is running on the user's computer and the SDK establishes a connection
        ///  to it, this function will return the user that is currently logged in to the Discord app.
        /// </summary>
        public void GetDiscordClientConnectedUser(
            ulong applicationId,
            GetDiscordClientConnectedUserCallback callback)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client
                    .GetDiscordClientConnectedUserCallback __callbackDelegate =
                        NativeMethods.Client.GetDiscordClientConnectedUserCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetDiscordClientConnectedUser(
                        self,
                        applicationId,
                        __callbackDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }

        /// <summary>
        ///  Returns the UserHandle associated with the given user ID.
        /// </summary>
        /// <remarks>
        ///  It will not fetch a user from Discord's API if it is not available. Generally you can trust
        ///  that users will be available for all relationships and for the authors of any messages
        ///  received.
        ///
        /// </remarks>
        public UserHandle? GetUser(ulong userId)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.UserHandle();
                UserHandle? __returnValue = null;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    __returnIsNonNull =
                        NativeMethods.Client.GetUser(self, userId, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new UserHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  The RelationshipGroupsUpdatedCallback is invoked whenever any user in the friends list
        ///  changes. This is intended to be the callback used to ensure the friends list is kept fresh.
        ///  This can be used in tandem with Client::GetRelationshipsByGroup to build and update the
        ///  friends list.
        /// </summary>
        public void SetRelationshipGroupsUpdatedCallback(
            RelationshipGroupsUpdatedCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.RelationshipGroupsUpdatedCallback __cbDelegate =
                    NativeMethods.Client.RelationshipGroupsUpdatedCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetRelationshipGroupsUpdatedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }

        /// <summary>
        ///  The UserUpdatedCallback is invoked whenever *any* user the current session knows about
        ///  changes, not just if the current user changes. For example if one of your Discord friends
        ///  changes their name or avatar the UserUpdatedCallback will be invoked. It is also invoked
        ///  when users come online, go offline, or start playing your game.
        /// </summary>
        public void SetUserUpdatedCallback(UserUpdatedCallback cb)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(Client));
            }

            unsafe
            {
                Discord.Sdk.NativeMethods.Client.UserUpdatedCallback __cbDelegate =
                    NativeMethods.Client.UserUpdatedCallback_Handler;

                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SetUserUpdatedCallback(
                        self,
                        __cbDelegate,
                        NativeMethods.ManagedUserData.Free,
                        NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }
    }

    /// <summary>
    ///  Convenience class that represents the state of a single Discord call in a lobby.
    /// </summary>
    public class CallInfoHandle : IDisposable
    {
        internal NativeMethods.CallInfoHandle self;
        private int disposed_;

        internal CallInfoHandle(NativeMethods.CallInfoHandle self, int disposed)
        {
            this.self = self;
            disposed_ = disposed;
        }

        ~CallInfoHandle() { Dispose(); }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed_, 1) != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);

            unsafe
            {
                fixed (NativeMethods.CallInfoHandle* self = &this.self)
                {
                    NativeMethods.CallInfoHandle.Drop(self);
                }
            }
        }

        public CallInfoHandle(CallInfoHandle other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(CallInfoHandle));
            }

            if (other.disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            unsafe
            {
                fixed (NativeMethods.CallInfoHandle* otherPtr = &other.self)
                {
                    fixed (NativeMethods.CallInfoHandle* selfPtr = &self)
                    {
                        NativeMethods.CallInfoHandle.Clone(selfPtr, otherPtr);
                    }
                }
            }
        }

        internal unsafe CallInfoHandle(NativeMethods.CallInfoHandle* otherPtr)
        {
            unsafe
            {
                fixed (NativeMethods.CallInfoHandle* selfPtr = &self)
                {
                    NativeMethods.CallInfoHandle.Clone(selfPtr, otherPtr);
                }
            }
        }

        /// <summary>
        ///  Returns the lobby ID of the call.
        /// </summary>
        public ulong ChannelId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(CallInfoHandle));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.CallInfoHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.CallInfoHandle.ChannelId(self);
                }

                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns a list of the user IDs of the participants in the call.
        /// </summary>
        public ulong[] GetParticipants()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(CallInfoHandle));
            }

            unsafe
            {
                var __returnValue = new NativeMethods.Discord_UInt64Span();

                fixed (NativeMethods.CallInfoHandle* self = &this.self)
                {
                    NativeMethods.CallInfoHandle.GetParticipants(self, &__returnValue);
                }

                var __returnValueSurface =
                    new Span<ulong>(__returnValue.ptr, (int)__returnValue.size).ToArray();
                NativeMethods.Discord_Free(__returnValue.ptr);
                return __returnValueSurface;
            }
        }

        /// <summary>
        ///  Accesses the voice state for a single user so you can know if they have muted or deafened
        ///  themselves.
        /// </summary>
        public VoiceStateHandle? GetVoiceStateHandle(ulong userId)
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(CallInfoHandle));
            }

            unsafe
            {
                bool __returnIsNonNull;
                var __returnValueNative = new NativeMethods.VoiceStateHandle();
                VoiceStateHandle? __returnValue = null;

                fixed (NativeMethods.CallInfoHandle* self = &this.self)
                {
                    __returnIsNonNull = NativeMethods.CallInfoHandle.GetVoiceStateHandle(
                        self, userId, &__returnValueNative);
                }

                if (!__returnIsNonNull)
                {
                    return null;
                }

                __returnValue = new VoiceStateHandle(__returnValueNative, 0);
                return __returnValue;
            }
        }

        /// <summary>
        ///  Returns the lobby ID of the call.
        /// </summary>
        public ulong GuildId()
        {
            if (disposed_ != 0)
            {
                throw new ObjectDisposedException(nameof(CallInfoHandle));
            }

            unsafe
            {
                ulong __returnValue;

                fixed (NativeMethods.CallInfoHandle* self = &this.self)
                {
                    __returnValue = NativeMethods.CallInfoHandle.GuildId(self);
                }

                return __returnValue;
            }
        }
    }
}
