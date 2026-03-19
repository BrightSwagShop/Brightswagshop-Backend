function createMugPayload() {
  const unique = Date.now();

  return {
    $type: 'Mok',
    name: `Playwright Mug ${unique}`,
    description: 'Simple API test product',
    price: 9.99,
    category: 'Drinkartikelen',
    productType: 'Mok',
    isActive: true,
    kleuren: [
      {
        kleur: 'Zwart',
        imageUrl: 'https://example.com/mug-black.png',
        stock: 10,
        sku: `MUG-${unique}`
      }
    ]
  };
}

module.exports = {
  createMugPayload
};
