const crypto = require('node:crypto');
const { request } = require('@playwright/test');

const BASE_URL = process.env.API_BASE_URL || 'http://127.0.0.1:5076';

async function createHeaderAuthContext(role = 'anonymous') {
  const headers = {};

  if (role !== 'anonymous') {
    headers['X-User-Id'] = `test-${role}-user`;
  }

  if (role === 'admin') {
    headers['X-User-Role'] = 'Admin';
  } else if (role === 'user') {
    headers['X-User-Role'] = 'User';
  }

  return request.newContext({
    baseURL: BASE_URL,
    extraHTTPHeaders: headers
  });
}

async function readApiResponse(response) {
  const contentType = response.headers()['content-type'] || '';
  const isJson = contentType.includes('application/json') || contentType.includes('+json');

  if (isJson) {
    const body = await response.json().catch(() => null);
    return { response, body };
  }

  return { response, body: await response.text() };
}

function createValidMugPayload({ name, imageUrl } = {}) {
  const unique = Date.now();

  return {
    $type: 'Mok',
    name: name || `api-mug-${unique}`,
    description: 'API test mug',
    price: 19.95,
    category: 'Drinkartikelen',
    productType: 'Mok',
    isActive: true,
    kleuren: [
      {
        kleur: 'Black',
        imageUrl: imageUrl || 'https://example.invalid/mug.jpg',
        stock: 5,
        sku: `MUG-${unique}`
      }
    ]
  };
}

function createValidDiscountPayload({ code, name, percentage = 20, startsAt, endsAt } = {}) {
  const now = new Date();

  return {
    name: name || `discount-${Date.now()}`,
    description: 'API test discount',
    percentage,
    code: code || `DISC-${Date.now()}`,
    startsAt: startsAt || new Date(now.getTime() - 60 * 60 * 1000).toISOString(),
    endsAt: endsAt || new Date(now.getTime() + 24 * 60 * 60 * 1000).toISOString(),
    isActive: true
  };
}

function createInvalidDateDiscountPayload() {
  const now = new Date();

  return {
    name: 'invalid-date-discount',
    description: 'Invalid date discount',
    percentage: 10,
    code: `BADDATE-${Date.now()}`,
    startsAt: new Date(now.getTime() + 24 * 60 * 60 * 1000).toISOString(),
    endsAt: new Date(now.getTime() - 60 * 60 * 1000).toISOString(),
    isActive: true
  };
}

function createCartRequest({ userId, sessionId = null, productId, quantity }) {
  return {
    userId,
    sessionId,
    items: [
      {
        productId,
        quantity,
        selectedColor: null
      }
    ]
  };
}

function createOrderRequest({ userId, items }) {
  return {
    userId,
    items
  };
}

function createMultipartFile({ fileType = 'valid image', fileName = 'upload.jpg' } = {}) {
  if (fileType === 'invalid type') {
    return {
      name: 'upload.txt',
      mimeType: 'text/plain',
      buffer: Buffer.from('not an image')
    };
  }

  if (fileType === 'zero-byte') {
    return {
      name: 'empty.jpg',
      mimeType: 'image/jpeg',
      buffer: Buffer.alloc(0)
    };
  }

  return {
    name: fileName,
    mimeType: 'image/jpeg',
    buffer: Buffer.from([0xff, 0xd8, 0xff, 0xd9])
  };
}

function buildStripeWebhookEvent({ orderId, sessionId, type = 'checkout.session.completed' } = {}) {
  return {
    id: `evt_${crypto.randomUUID().replace(/-/g, '')}`,
    object: 'event',
    type,
    data: {
      object: {
        id: sessionId || `cs_${crypto.randomUUID().replace(/-/g, '')}`,
        object: 'checkout.session',
        metadata: {
          OrderId: orderId
        }
      }
    }
  };
}

function signStripeWebhookPayload(payload, secret, timestamp = Math.floor(Date.now() / 1000)) {
  const body = typeof payload === 'string' ? payload : JSON.stringify(payload);
  const signature = crypto
    .createHmac('sha256', secret)
    .update(`${timestamp}.${body}`, 'utf8')
    .digest('hex');

  return `t=${timestamp},v1=${signature}`;
}

function base64Url(value) {
  return Buffer.from(JSON.stringify(value))
    .toString('base64')
    .replace(/=/g, '')
    .replace(/\+/g, '-')
    .replace(/\//g, '_');
}

function signJwtToken({ key, issuer, audience, subject, name, role, expiresInSeconds = 7200 }) {
  const header = {
    alg: 'HS256',
    typ: 'JWT'
  };

  const now = Math.floor(Date.now() / 1000);
  const payload = {
    iss: issuer,
    aud: audience,
    sub: subject,
    nameid: subject,
    unique_name: name,
    role,
    nbf: now,
    exp: now + expiresInSeconds,
    iat: now
  };

  const encodedHeader = Buffer.from(JSON.stringify(header)).toString('base64url');
  const encodedPayload = base64Url(payload);
  const unsignedToken = `${encodedHeader}.${encodedPayload}`;
  const signature = crypto
    .createHmac('sha256', key)
    .update(unsignedToken)
    .digest('base64url');

  return `${unsignedToken}.${signature}`;
}

module.exports = {
  BASE_URL,
  createHeaderAuthContext,
  readApiResponse,
  createValidMugPayload,
  createValidDiscountPayload,
  createInvalidDateDiscountPayload,
  createCartRequest,
  createOrderRequest,
  createMultipartFile,
  buildStripeWebhookEvent,
  signStripeWebhookPayload,
  signJwtToken
};