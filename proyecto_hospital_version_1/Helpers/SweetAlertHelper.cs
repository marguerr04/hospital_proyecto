using Microsoft.JSInterop;

namespace proyecto_hospital_version_1.Helpers
{
    public class SweetAlertHelper
    {
        private readonly IJSRuntime _jsRuntime;

        public SweetAlertHelper(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task Success(string title, string message = "")
        {
            await _jsRuntime.InvokeVoidAsync("sweetAlert.success", title, message);
        }

        public async Task Error(string title, string message = "")
        {
            await _jsRuntime.InvokeVoidAsync("sweetAlert.error", title, message);
        }

        public async Task Warning(string title, string message = "")
        {
            await _jsRuntime.InvokeVoidAsync("sweetAlert.warning", title, message);
        }

        public async Task Info(string title, string message = "")
        {
            await _jsRuntime.InvokeVoidAsync("sweetAlert.info", title, message);
        }

        public async Task ValidationError(string title, List<string> errors)
        {
            await _jsRuntime.InvokeVoidAsync("sweetAlert.validationError", title, errors);
        }
    }
}