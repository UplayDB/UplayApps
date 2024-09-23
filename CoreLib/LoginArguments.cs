namespace CoreLib
{
    public struct LoginArguments 
    {
        public string Base64Login;
        public string AccountId;
        public string Email;
        public string Password;
        public bool rememberMe;
        public bool rememberDevice;
        public string trustedDeviceId;
        public string trustedDeviceName;
        public bool useFileStorage;
        public bool printStorage;

        public override readonly string ToString() => $"B64: {Base64Login}, AID: {AccountId}, Email: {Email}, Password L: {Password.Length}\n" +
                $"RM: {rememberMe}, RD: {rememberDevice}, TDID: {trustedDeviceId}, TDNAME: {trustedDeviceName}, UFS: {useFileStorage}";
    }
}
