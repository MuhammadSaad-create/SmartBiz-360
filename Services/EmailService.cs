using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace SmartBiz360.Services
{
    public class EmailService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly bool _isConfigured;

        public EmailService(IConfiguration config)
        {
            _apiKey = config["Brevo:ApiKey"] ?? "";
            _fromEmail = config["Brevo:FromEmail"] ?? "";
            _fromName = config["Brevo:FromName"] ?? "SmartBiz 360";
            _isConfigured = !string.IsNullOrEmpty(_apiKey) &&
                            !string.IsNullOrEmpty(_fromEmail);
        }

        private async Task<bool> SendAsync(
            string toEmail, string toName,
            string subject, string body)
        {
            if (!_isConfigured)
            {
                Console.WriteLine(
                    $"[EMAIL SIMULATION] To:{toEmail} Sub:{subject}");
                return true;
            }

            try
            {
                using var http = new HttpClient();
                http.DefaultRequestHeaders.Add("api-key", _apiKey);

                var payload = new Dictionary<string, object>
                {
                    ["sender"] = new Dictionary<string, string>
                    {
                        ["email"] = _fromEmail,
                        ["name"] = _fromName
                    },
                    ["to"] = new[]
                    {
                        new Dictionary<string, string>
                        {
                            ["email"] = toEmail,
                            ["name"] = toName
                        }
                    },
                    ["subject"] = subject,
                    ["htmlContent"] = body
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(
                    json, Encoding.UTF8, "application/json");

                var response = await http.PostAsync(
                    "https://api.brevo.com/v3/smtp/email", content);

                var responseBody = await response.Content
                    .ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[EMAIL SENT] To:{toEmail}");
                    return true;
                }

                Console.WriteLine(
                    $"[EMAIL FAILED] {response.StatusCode}" +
                    $" | {responseBody}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] {ex.Message}");
                return false;
            }
        }

        private string BaseTemplate(string content)
        {
            return "<div style=\"font-family:Arial,sans-serif;" +
                   "max-width:600px;margin:0 auto;\">" +
                   "<div style=\"background:#1e3a8a;padding:20px;" +
                   "text-align:center;\">" +
                   "<h1 style=\"color:white;margin:0;\">SmartBiz 360" +
                   "</h1></div>" +
                   "<div style=\"padding:30px;\">" +
                   content +
                   "</div>" +
                   "<div style=\"background:#f3f4f6;padding:15px;" +
                   "text-align:center;font-size:12px;color:#6b7280;\">" +
                   "SmartBiz 360 - Precision Analytics" +
                   "</div></div>";
        }

        public async Task SendWelcomeEmailAsync(
            string email, string name, string password)
        {
            var content = $"<h2>Welcome, {name}!</h2>" +
                $"<p>Your account has been created.</p>" +
                $"<p><b>Email:</b> {email}</p>" +
                $"<p><b>Password:</b> {password}</p>" +
                $"<p>Please login and change your password.</p>";

            await SendAsync(email, name,
                "Welcome to SmartBiz 360",
                BaseTemplate(content));
        }

        public async Task SendDealUpdateAsync(
            string email, string name,
            string dealName, string newStage)
        {
            var content = $"<h2>Deal Stage Updated</h2>" +
                $"<p>Hi {name},</p>" +
                $"<p>Your deal <b>{dealName}</b> moved to " +
                $"<b>{newStage}</b>.</p>";

            await SendAsync(email, name,
                $"Deal Update: {dealName} to {newStage}",
                BaseTemplate(content));
        }

        public async Task SendAtRiskAlertAsync(
            string email, string repName,
            string companyName, int daysSinceContact)
        {
            var content = $"<h2>Customer At Risk</h2>" +
                $"<p>Hi {repName},</p>" +
                $"<p><b>{companyName}</b> has not been contacted " +
                $"in <b>{daysSinceContact} days</b>.</p>" +
                $"<p>Please reach out to prevent churn.</p>";

            await SendAsync(email, repName,
                $"At-Risk Customer: {companyName}",
                BaseTemplate(content));
        }

        public async Task SendOverdueTaskAlertAsync(
            string email, string name, string taskTitle)
        {
            var content = $"<h2>Task Overdue</h2>" +
                $"<p>Hi {name},</p>" +
                $"<p>Your task <b>{taskTitle}</b> is overdue." +
                $" Please update it.</p>";

            await SendAsync(email, name,
                $"Overdue Task: {taskTitle}",
                BaseTemplate(content));
        }

        public async Task SendProductivityDigestAsync(
            string email, string managerName,
            double teamAverage, int totalScored)
        {
            var content = $"<h2>Daily Productivity Report</h2>" +
                $"<p>Hi {managerName},</p>" +
                $"<p>Team Average Score: <b>{teamAverage:F1}</b></p>" +
                $"<p>Employees Scored: <b>{totalScored}</b></p>";

            await SendAsync(email, managerName,
                "Daily Productivity Digest",
                BaseTemplate(content));
        }

        public async Task SendCampaignEmailAsync(
            string email, string campaignName,
            string targetAudience)
        {
            var content = $"<h2>{campaignName}</h2>" +
                $"<p>This is a marketing communication " +
                $"from SmartBiz 360.</p>";

            await SendAsync(email, targetAudience,
                campaignName,
                BaseTemplate(content));
        }

        public async Task SendReportEmailAsync(
            string email, string name, string reportName)
        {
            var content = $"<h2>Your Report is Ready</h2>" +
                $"<p>Hi {name},</p>" +
                $"<p>Your <b>{reportName}</b> has been generated." +
                $" Login to download it.</p>";

            await SendAsync(email, name,
                $"Report Ready: {reportName}",
                BaseTemplate(content));
        }
        public async Task SendTaskReportEmailAsync(
            string email, string name,
            string grade, int completed, int total,
            double completionRate, double onTimeRate,
            int overdue, bool bonusEligible, DateTime generatedAt)
        {
            var gradeColor = grade switch
            {
                "Excellent" => "#16a34a",
                "Average" => "#ca8a04",
                "Warning" => "#ea580c",
                "Serious Warning" => "#dc2626",
                _ => "#6b7280"
            };

            var bonusText = bonusEligible
                ? "<span style=\"background:#16a34a;color:white;padding:4px 14px;" +
                  "border-radius:20px;font-size:13px;\">✅ Bonus Eligible</span>"
                : "<span style=\"background:#6b7280;color:white;padding:4px 14px;" +
                  "border-radius:20px;font-size:13px;\">❌ Not Bonus Eligible</span>";

            var content =
                $"<h2 style=\"color:#1e3a8a;\">Task Performance Report</h2>" +
                $"<p>Hi <b>{name}</b>,</p>" +
                $"<p>Your task performance report has been generated. Here is a summary:</p>" +
                $"<div style=\"background:#f8fafc;border-radius:8px;padding:20px;margin:16px 0;\">" +
                $"<table style=\"width:100%;border-collapse:collapse;\">" +
                $"<tr><td style=\"padding:8px 0;color:#6b7280;\">Total Tasks</td>" +
                $"<td style=\"padding:8px 0;font-weight:600;\">{total}</td></tr>" +
                $"<tr><td style=\"padding:8px 0;color:#6b7280;\">Completed</td>" +
                $"<td style=\"padding:8px 0;font-weight:600;color:#16a34a;\">{completed}</td></tr>" +
                $"<tr><td style=\"padding:8px 0;color:#6b7280;\">Overdue</td>" +
                $"<td style=\"padding:8px 0;font-weight:600;color:#dc2626;\">{overdue}</td></tr>" +
                $"<tr><td style=\"padding:8px 0;color:#6b7280;\">Completion Rate</td>" +
                $"<td style=\"padding:8px 0;font-weight:600;\">{completionRate}%</td></tr>" +
                $"<tr><td style=\"padding:8px 0;color:#6b7280;\">On-Time Rate</td>" +
                $"<td style=\"padding:8px 0;font-weight:600;\">{onTimeRate}%</td></tr>" +
                $"</table></div>" +
                $"<div style=\"text-align:center;margin:24px 0;\">" +
                $"<p style=\"font-size:13px;color:#6b7280;margin-bottom:4px;\">Performance Grade</p>" +
                $"<div style=\"font-size:30px;font-weight:700;color:{gradeColor};\">{grade}</div>" +
                $"<div style=\"margin-top:10px;\">{bonusText}</div>" +
                $"</div>" +
                $"<p style=\"color:#9ca3af;font-size:12px;\">Generated on {generatedAt:MMMM dd, yyyy hh:mm tt}</p>";

            await SendAsync(email, name,
                $"Your Task Performance Report — {grade}",
                BaseTemplate(content));
        }


        public async Task SendPasswordResetEmailAsync(
            string email, string name, string token, string baseUrl)
        {
            // Use the actual running base URL passed from the page
            var resetUrl = $"{baseUrl.TrimEnd('/')}/reset-password?token={token}";

            var content =
                $"<h2 style=\"color:#1e3a8a;\">Password Reset Request</h2>" +
                $"<p>Hi <b>{name}</b>,</p>" +
                $"<p>We received a request to reset your SmartBiz 360 password.</p>" +
                $"<div style=\"text-align:center;margin:28px 0;\">" +
                $"<a href=\"{resetUrl}\" " +
                $"style=\"background:#1e3a8a;color:white;padding:12px 28px;" +
                $"border-radius:6px;text-decoration:none;font-weight:600;" +
                $"font-size:15px;\">" +
                $"Reset My Password" +
                $"</a></div>" +
                $"<p style=\"color:#6b7280;font-size:13px;\">" +
                $"This link expires in <b>1 hour</b>. " +
                $"If you did not request this, ignore this email — " +
                $"your password will not change.</p>" +
                $"<p style=\"color:#9ca3af;font-size:12px;word-break:break-all;\">" +
                $"Or copy this link: {resetUrl}</p>";

            await SendAsync(email, name,
                "Reset Your SmartBiz 360 Password",
                BaseTemplate(content));
        }

    }
}
