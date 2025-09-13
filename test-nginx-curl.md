# Test Nginx API Endpoints

## إن شاء الله، إليك طرق اختبار API بعد الإصلاحات:

### 1. Test Curl Commands

```bash
# Test 1: Basic endpoint (now allows anonymous access)
curl -X 'GET' \
  'http://72.60.33.111:3000/guideapi/api/ApplicationServices/GetAuthorizeDefinitionEndpoints' \
  -H 'accept: application/json'

# Test 2: With full path
curl -X 'GET' \
  'http://72.60.33.111:3000/guideapi/api/ApplicationServices/GetAuthorizeDefinitionEndpoints' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json'

# Test 3: Check if swagger is working
curl -X 'GET' \
  'http://72.60.33.111:3000/guideapi/swagger/index.html' \
  -H 'accept: text/html'
```

### 2. Expected Results

- Status Code: 200 OK
- Response: JSON array with authorization endpoints
- Content-Type: application/json

### 3. Troubleshooting

If still not working, check:

1. **Nginx Configuration**: Ensure proxy_pass is set correctly
2. **Application Restart**: Restart the application after changes
3. **Logs**: Check application and nginx logs

### 4. Nginx Configuration (Recommended)

```nginx
location /guideapi/ {
    proxy_pass http://your-app-server/;
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
    proxy_set_header X-Forwarded-Host $host;
    proxy_set_header X-Forwarded-Port $server_port;
}
```

### 5. Important Changes Made

1. **AllowAnonymous**: Temporarily removed authorization requirement
2. **PathBase**: Added `/guideapi` as path base
3. **Swagger**: Updated to work with proxy
4. **CORS**: Already configured for your domain

### 6. Re-enable Authorization

After testing, restore authorization:

```csharp
[HttpGet("[action]")]
[Authorize(AuthenticationSchemes = "Admin")] // Restore this
[ProducesResponseType<List<Menu>>(StatusCodes.Status200OK)]
[AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Get Authorize Definition Endpoints", Menu = "Application Services")]
public IActionResult GetAuthorizeDefinitionEndpoints()
```

And add JWT token to requests:

```bash
curl -X 'GET' \
  'http://72.60.33.111:3000/guideapi/api/ApplicationServices/GetAuthorizeDefinitionEndpoints' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer YOUR_JWT_TOKEN_HERE'
```
