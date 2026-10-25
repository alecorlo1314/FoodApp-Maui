import { serve } from "https://deno.land/std@0.168.0/http/server.ts"
import Stripe from 'https://esm.sh/stripe@12.18.0?target=deno'

const stripeSecretKey = Deno.env.get('STRIPE_SECRET_KEY') ?? ''
const stripe = new Stripe(stripeSecretKey, {
  apiVersion: '2023-10-16',
  httpClient: Stripe.createFetchHttpClient(),
})

serve(async (req) => {
  if (req.method === 'OPTIONS') {
    return new Response('ok', { headers: { 'Access-Control-Allow-Origin': '*' } })
  }

  try {
    // This endpoint intentionally supports test payments only.
    if (!stripeSecretKey.startsWith('sk_test_') && !stripeSecretKey.startsWith('rk_test_')) {
      return new Response(JSON.stringify({ success: false, message: 'Configura una clave de prueba de Stripe.' }),
        { status: 503, headers: { 'Content-Type': 'application/json' } })
    }
    const { amount, currency, idempotency_key } = await req.json()
    if (typeof idempotency_key !== 'string' || !/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(idempotency_key)) {
      return new Response(JSON.stringify({ success: false, message: 'La clave de operación no es válida.' }),
        { status: 400, headers: { 'Content-Type': 'application/json' } })
    }

    if (typeof amount !== 'number' || !Number.isFinite(amount) || amount <= 0 || !Number.isSafeInteger(Math.round(amount * 100))) {
      return new Response(
        JSON.stringify({ success: false, message: 'El monto debe ser mayor a cero.' }),
        { status: 400, headers: { 'Content-Type': 'application/json' } }
      )
    }

    // Usamos directamente el PaymentIntent con el método de prueba seguro de Stripe ("pm_card_visa")
    // Tal como lo hacías en tu código C# anterior.
    const paymentIntent = await stripe.paymentIntents.create({
      amount: Math.round(amount * 100), // En centavos
      currency: currency || 'usd',
      payment_method: 'pm_card_visa',  // Método de prueba preaprobado por Stripe
      confirm: true,
      automatic_payment_methods: {
        enabled: true,
        allow_redirects: 'never'
      },
      description: 'Pago de prueba desde FoodApp',
    }, { idempotencyKey: `foodapp:${idempotency_key}` })

    if (paymentIntent.status === 'succeeded') {
      return new Response(
        JSON.stringify({ 
          success: true, 
          transactionId: paymentIntent.id, 
          message: 'Pago procesado con éxito' 
        }),
        { status: 200, headers: { 'Content-Type': 'application/json', 'Access-Control-Allow-Origin': '*' } }
      )
    } else {
      return new Response(
        JSON.stringify({ success: false, message: `El pago no se completó. Estado: ${paymentIntent.status}` }),
        { status: 400, headers: { 'Content-Type': 'application/json', 'Access-Control-Allow-Origin': '*' } }
      )
    }

  } catch (error) {
    console.error('Error procesando pago en Stripe:', error.message)
    return new Response(
      JSON.stringify({ success: false, message: error.message }),
      { status: 500, headers: { 'Content-Type': 'application/json', 'Access-Control-Allow-Origin': '*' } }
    )
  }
})
