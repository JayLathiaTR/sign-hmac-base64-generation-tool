function $(id){ return document.getElementById(id); }

function setError(msg){
  const el = $('error');
  el.textContent = msg || '';
}

function toBase64(bytes){
  let binary = '';
  const arr = new Uint8Array(bytes);
  for (let i = 0; i < arr.length; i++) binary += String.fromCharCode(arr[i]);
  return btoa(binary);
}

async function hmacSha256Base64(secretKey, message){
  const enc = new TextEncoder();
  const keyData = enc.encode(secretKey);
  const msgData = enc.encode(message);

  const key = await crypto.subtle.importKey(
    'raw',
    keyData,
    { name: 'HMAC', hash: 'SHA-256' },
    false,
    ['sign']
  );

  const sig = await crypto.subtle.sign('HMAC', key, msgData);
  return toBase64(sig);
}

function buildAitJson(){
  const sapSoldToAccount = $('aitSapSoldToAccount').value.trim();
  const emsIdText = $('aitEmsId').value.trim();
  const emFirmId = $('aitEmFirmId').value.trim();
  const productDomain = $('aitProductDomain').value.trim();

  if (!sapSoldToAccount || !emsIdText || !emFirmId || !productDomain)
    throw new Error('All AIT fields are required.');

  if (!/^\d+$/.test(emsIdText))
    throw new Error('AIT emsId must be a number (e.g., 199972).');

  const obj = {
    sapSoldToAccount: sapSoldToAccount,
    emsId: Number(emsIdText),
    emFirmId: emFirmId,
    productDomain: productDomain
  };

  return JSON.stringify(obj);
}

function buildAiaJson(){
  const emsId = $('aiaEmsId').value.trim();
  const sapSoldToAccount = $('aiaSapSoldToAccount').value.trim();
  const emFirmId = $('aiaEmFirmId').value.trim();

  if (!emsId || !sapSoldToAccount || !emFirmId)
    throw new Error('All AIA fields are required.');

  const obj = {
    EmsId: emsId,
    SapSoldToAccount: sapSoldToAccount,
    EmFirmId: emFirmId
  };

  return JSON.stringify(obj);
}

function syncSections(){
  const product = $('product').value;
  const ait = $('aitSection');
  const aia = $('aiaSection');
  if (product === 'AIT') {
    ait.hidden = false;
    aia.hidden = true;
  } else {
    ait.hidden = true;
    aia.hidden = false;
  }
}

async function generate(){
  setError('');
  $('rawBody').value = '';
  $('hmac').value = '';

  const secretKey = $('secretKey').value;
  if (!secretKey || !secretKey.trim()){
    setError('SecretKey is required.');
    return;
  }

  try {
    const product = $('product').value;
    const rawBody = (product === 'AIT') ? buildAitJson() : buildAiaJson();
    $('rawBody').value = rawBody;

    const h = await hmacSha256Base64(secretKey, rawBody);
    $('hmac').value = h;
  } catch (e) {
    setError(e && e.message ? e.message : String(e));
  }
}

async function copyText(text){
  if (!text) return;
  if (navigator.clipboard && navigator.clipboard.writeText){
    await navigator.clipboard.writeText(text);
    return;
  }
  // fallback
  const ta = document.createElement('textarea');
  ta.value = text;
  document.body.appendChild(ta);
  ta.select();
  document.execCommand('copy');
  document.body.removeChild(ta);
}

window.addEventListener('DOMContentLoaded', () => {
  syncSections();
  $('product').addEventListener('change', syncSections);
  $('generate').addEventListener('click', generate);
  $('copyRaw').addEventListener('click', () => copyText($('rawBody').value));
  $('copyHmac').addEventListener('click', () => copyText($('hmac').value));
});
