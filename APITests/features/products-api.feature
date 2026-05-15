Feature: Products API CRUD validation
  As a user
  I want to view Products
  And add them to my cart
  So that i can shop on the website.

  @HF
	Scenario: Listing all products returns a populated collection partition
		When I GET "/api/products"
		Then the response status should be 200
		And the response should be an array of products

  
	Scenario Outline: Product lookup by id follows existing and missing id partitions
		When I GET "/api/products/<productId>"
		Then the response status should be <status>

		Examples:
			| productId                | status |
			| existing-product-id      | 200    |
			| 000000000000000000000000 | 404    |

  
	Scenario Outline: Product lookup by type follows known and unknown slug partitions
		When I GET "/api/products/type/<slug>"
		Then the response status should be <status>

		Examples:
			| slug    | status |
			| tshirt  | 200    |
			| hoodie  | 200    |
			| mok     | 200    |
			| sticker | 200    |
			| unknown | 404    |

  @HF
	Scenario: Categories endpoint returns the category shape partition
		When I GET "/api/categories"
		Then the response status should be 200
		And every returned category should contain a numeric id and a string name

  @HF
	Scenario: Product types endpoint returns the product type shape partition
		When I GET "/api/producttypes"
		Then the response status should be 200
		And every returned product type should contain a string name and a string slug
    
	Scenario Outline: Bulk product lookup treats an empty list and a single id as boundary values
		Given I prepare a product id list with <count> item(s)
		When I POST "/api/products/by-ids" with the id list
		Then the response status should be <status>

		Examples:
			| count | status |
			| 0     | 400    |
			| 1     | 200    |

	Scenario Outline: Image upload treats zero-byte and non-empty files as boundary values
		Given I prepare an upload file in the "<fileSize>" boundary state
		When I POST "/api/images/upload" as admin
		Then the response status should be <status>

		Examples:
			| fileSize  | status |
			| zero-byte | 400    |
			| minimal   | 200    |

	Scenario Outline: Order checkout treats empty and one-item orders as boundary values
		Given I prepare an order in the "<itemCount>" boundary state
		When I POST "/api/payments/<orderId>/checkout"
		Then the response status should be <status>

		Examples:
			| itemCount  | orderId              | status |
			| zero-items  | empty-order-id      | 400    |
			| one-item    | single-item-order-id | 200    |

	Scenario Outline: Header authentication treats missing and minimal user id values as boundary values
		Given I call a protected endpoint with X-User-Id value "<userIdHeader>"
		When I GET "/api/debug/claims"
		Then the response status should be <status>

		Examples:
			| userIdHeader | status |
			| missing      | 401    |
			| whitespace   | 401    |
			| u            | 200    |

  Scenario: Product create payload with invalid enum/type values is rejected
    Given I am authenticated as admin
    When I POST "/api/products" with a payload that has productType "INVALID_TYPE"
    Then the response status should be 400
    And the response should describe the invalid enum/value