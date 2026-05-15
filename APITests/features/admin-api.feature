Feature: Admin authorization partitions
	As a product owner
	I want role-based endpoints to distinguish public users, and admin access
	So that privileged actions are blocked from the wrong audience

	@HF
	Scenario: An admin uploads an image and publishes a product
		Given I am authenticated as an admin user
		And I have a valid image file
		And I have a valid product payload
		When I POST "/api/images/upload" with the image file as admin
		Then the response status should be 200
		And the response should contain an imageUrl
		When I POST "/api/products" with the product payload as admin
		Then the response status should be 201
		And the created product should be retrievable by id
		When I DELETE the created product by id as admin
		Then the response status should be 204

	@HF
	Scenario: An admin creates and updates a discount definition
		Given I am authenticated as an admin user
		And I have a valid discount payload
		When I POST "/api/discounts" with the discount payload as admin
		Then the response status should be 201
		And I remember the created discount id
		When I PUT "/api/discounts/{discountId}" with an updated discount payload
		Then the response status should be 200
		And the response should reflect the updated discount values

	@HF
	Scenario Outline: Product write operations follow role partitions
		Given I call the products endpoint with the "<auth>" authentication partition
		When I POST "/api/products" with a valid product payload
		Then the response status should be <createStatus>
		When I DELETE "/api/products/existing-product-id" with the same authentication partition
		Then the response status should be <deleteStatus>

		Examples:
			| auth         | createStatus | deleteStatus |
			| public user  | 403          | 403          |
			| admin user    | 201          | 204          |

	@HF
	Scenario Outline: Image upload follows role and file-type partitions
		Given I call the image upload endpoint with the "<auth>" authentication partition
		And I attach a file in the "<fileType>" partition
		When I POST "/api/images/upload"
		Then the response status should be <status>

		Examples:
			| auth         | fileType     | status |
			| public user | valid image  | 403    |
			| admin user   | valid image  | 200    |
			| admin user   | invalid type  | 400    |

	@HF
	Scenario Outline: Discount creation follows role partitions
		Given I call the discounts endpoint with the "<auth>" authentication partition
		When I POST "/api/discounts" with a valid discount payload
		Then the response status should be <status>

		Examples:
			| auth         | status |
			| public user | 403    |
			| admin user   | 201    |

	@HF
	Scenario Outline: Debug and admin-only endpoints follow authentication partitions
		Given I call the protected support endpoint with the "<auth>" authentication partition
		When I GET the protected endpoint
		Then the response status should be <status>

		Examples:
			| auth                  | status |
			| anonymous             | 401    |
			| valid HeaderAuth user | 200    |
			| valid HeaderAuth admin | 200    |
			| wrong role for admin api | 401  |

	@HF
	Scenario Outline: Stripe webhook handling follows request shape partitions
		Given I prepare a Stripe webhook request in the "<partition>" partition
		When I POST "/api/webhooks/stripe"
		Then the response status should be <status>

		Examples:
			| partition             | status |
			| missing webhook secret | 400    |
			| missing signature      | 400    |
			| invalid signature      | 400    |
			| valid event            | 200    |

	Scenario: Protected AzureAd admin endpoint denies alternative auth schemes
		Given I am authenticated via HeaderAuth with role "App.Admin"
		When I GET "/api/admins/admin-only" with the HeaderAuth credentials
		Then the response status should be 401