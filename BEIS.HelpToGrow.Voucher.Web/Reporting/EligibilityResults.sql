DROP TABLE IF EXISTS ecr_report
;

CREATE TEMP TABLE ecr_report (
	call_id bigint NOT NULL,
	company_name text,
	companies_house_number text,
	br01 text,
	br02 text,
	br03 text,
	br04 text,
	br05 text,
	br06 text,
	br07 text,
	br08 text,
	br09 text,
	br10 text,
	br11 text,
	br12 text,
	br13 text,
	br14 text,
	br15 text,
	br16 text,
	eligible text,
	spot_check text
)
;

-- Populate reporting table with enterprise details and indesser call api results

INSERT INTO ecr_report (call_id, company_name, companies_house_number)
SELECT I.call_id, E.enterprise_name, E.companies_house_no
FROM indesser_api_call_status I
JOIN enterprise E ON E.enterprise_id = I.enterprise_id
WHERE E.companies_house_no IS NOT null
;

DROP TABLE IF EXISTS ecr
;

-- Create temp table for storing JSON responses as JSON datatype

CREATE TEMP TABLE ecr (
    eligibility_check_result_id bigint NOT NULL,
    call_id bigint NOT NULL,
    passed_check boolean NOT NULL,
    spot_check_object JSONB,
    result_object JSONB,
    result_datetime timestamp without time zone NOT NULL
)
;

INSERT INTO ecr
SELECT 
	eligibility_check_result_id,
	call_id,
	passed_check,
	CAST(spot_check_object AS json),
	CAST(result_object AS json),
	result_datetime
FROM
	eligibility_check_result
;

-- Update eligibility & spot check requirement

UPDATE ecr_report
SET eligible = CASE WHEN passed_check = 'true' THEN 'Y' ELSE null END,
	spot_check = CASE WHEN passed_check = 'true' AND spot_check_object IS NOT NULL THEN 'Y' ELSE null END
FROM ecr
WHERE ecr_report.call_id = ecr.call_id
;


-- BR01

UPDATE ecr_report
SET br01 = messages.error_message
FROM
 (SELECT
	call_id,
	result_object,
	CASE
		WHEN CAST(result_object::json -> 'Errors' -> 0 -> 'Message' AS text) LIKE '%UK postcode%' THEN result_object::json -> 'Errors' -> 0 -> 'Message'
	END AS error_message
  FROM ecr) AS messages
WHERE ecr_report.call_id = messages.call_id
AND messages.result_object @> '{"Errors":[{"ErrorCode": 1}]}'
;

-- BR02


UPDATE ecr_report
SET br02 = messages.error_message
FROM
 (SELECT
	call_id,
	result_object,
	CASE
		WHEN CAST(result_object::json -> 'Errors' -> 0 -> 'Message' AS text) LIKE '%trading history%' THEN result_object::json -> 'Errors' -> 0 -> 'Message'
		WHEN CAST(result_object::json -> 'Errors' -> 1 -> 'Message' AS text) LIKE '%trading history%' THEN result_object::json -> 'Errors' -> 1 -> 'Message'
	END AS error_message
  FROM ecr) AS messages
WHERE ecr_report.call_id = messages.call_id
AND messages.result_object @> '{"Errors":[{"ErrorCode": 2}]}'
;

-- BR03


UPDATE ecr_report
SET br03 = messages.error_message
FROM
 (SELECT
	call_id,
	result_object,
	CASE
		WHEN CAST(result_object::json -> 'Errors' -> 0 -> 'Message' AS text) LIKE '%trading status%' THEN result_object::json -> 'Errors' -> 0 -> 'Message'
		WHEN CAST(result_object::json -> 'Errors' -> 1 -> 'Message' AS text) LIKE '%trading status%' THEN result_object::json -> 'Errors' -> 1 -> 'Message'
		WHEN CAST(result_object::json -> 'Errors' -> 2 -> 'Message' AS text) LIKE '%trading status%' THEN result_object::json -> 'Errors' -> 2 -> 'Message'
	END AS error_message
  FROM ecr) AS messages
WHERE ecr_report.call_id = messages.call_id
AND messages.result_object @> '{"Errors":[{"ErrorCode": 3}]}'
;

-- BR14


UPDATE ecr_report
SET br14 = messages.error_message
FROM
 (SELECT
	call_id,
	result_object,
	CASE
		WHEN CAST(result_object::json -> 'Errors' -> 0 -> 'Message' AS text) LIKE '%Low protect%' THEN result_object::json -> 'Errors' -> 0 -> 'Message'
		WHEN CAST(result_object::json -> 'Errors' -> 1 -> 'Message' AS text) LIKE '%Low protect%' THEN result_object::json -> 'Errors' -> 1 -> 'Message'
		WHEN CAST(result_object::json -> 'Errors' -> 2 -> 'Message' AS text) LIKE '%Low protect%' THEN result_object::json -> 'Errors' -> 2 -> 'Message'
		WHEN CAST(result_object::json -> 'Errors' -> 3 -> 'Message' AS text) LIKE '%Low protect%' THEN result_object::json -> 'Errors' -> 3 -> 'Message'
	END AS error_message
  FROM ecr) AS messages
WHERE ecr_report.call_id = messages.call_id
AND messages.result_object @> '{"Errors":[{"ErrorCode": 14}]}'
;

-- BR04


UPDATE ecr_report
SET br04 = messages.error_message
FROM
 (SELECT
	call_id,
	result_object,
	CASE
		WHEN CAST(result_object::json -> 'RecordedItems' -> 0 -> 'Message' AS text) LIKE '%employees%' THEN result_object::json -> 'RecordedItems' -> 0 -> 'Message'
	END AS error_message
  FROM ecr) AS messages
WHERE ecr_report.call_id = messages.call_id
AND messages.result_object @> '{"RecordedItems":[{"ErrorCode": 4}]}'
;

-- BR05


UPDATE ecr_report
SET br05 = messages.error_message
FROM
 (SELECT
	call_id,
	result_object,
	CASE
		WHEN CAST(result_object::json -> 'ReviewItems' -> 0 -> 'Message' AS text) LIKE '%Gazette data%' THEN result_object::json -> 'ReviewItems' -> 0 -> 'Message'
		WHEN CAST(result_object::json -> 'ReviewItems' -> 1 -> 'Message' AS text) LIKE '%Gazette data%' THEN result_object::json -> 'ReviewItems' -> 1 -> 'Message'
		WHEN CAST(result_object::json -> 'ReviewItems' -> 2 -> 'Message' AS text) LIKE '%Gazette data%' THEN result_object::json -> 'ReviewItems' -> 2 -> 'Message'
	END AS error_message
  FROM ecr) AS messages
WHERE ecr_report.call_id = messages.call_id
AND messages.result_object @> '{"ReviewItems":[{"Characteristic": {"ErrorCode": 5} }]}'
;

-- BR06


UPDATE ecr_report
SET br06 = messages.error_message
FROM
 (SELECT
	call_id,
	result_object,
	CASE
		WHEN CAST(result_object::json -> 'RecordedItems' -> 0 -> 'Message' AS text) LIKE '%Financial agreement%' THEN result_object::json -> 'RecordedItems' -> 0 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 1 -> 'Message' AS text) LIKE '%Financial agreement%' THEN result_object::json -> 'RecordedItems' -> 1 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 2 -> 'Message' AS text) LIKE '%Financial agreement%' THEN result_object::json -> 'RecordedItems' -> 2 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 3 -> 'Message' AS text) LIKE '%Financial agreement%' THEN result_object::json -> 'RecordedItems' -> 3 -> 'Message'
	END AS error_message
  FROM ecr) AS messages
WHERE ecr_report.call_id = messages.call_id
AND messages.result_object @> '{"RecordedItems":[{"ErrorCode": 6}]}'
;

-- BR07


UPDATE ecr_report
SET br07 = messages.error_message
FROM
 (SELECT
	call_id,
	result_object,
	CASE
		WHEN CAST(result_object::json -> 'RecordedItems' -> 0 -> 'Message' AS text) LIKE '%Director disqualification%' THEN result_object::json -> 'RecordedItems' -> 0 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 1 -> 'Message' AS text) LIKE '%Director disqualification%' THEN result_object::json -> 'RecordedItems' -> 1 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 2 -> 'Message' AS text) LIKE '%Director disqualification%' THEN result_object::json -> 'RecordedItems' -> 2 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 3 -> 'Message' AS text) LIKE '%Director disqualification%' THEN result_object::json -> 'RecordedItems' -> 3 -> 'Message'
	END AS error_message
  FROM ecr) AS messages
WHERE ecr_report.call_id = messages.call_id
AND messages.result_object @> '{"RecordedItems":[{"ErrorCode": 7}]}'
;

-- BR08


UPDATE ecr_report
SET br08 = messages.error_message
FROM
 (SELECT
	call_id,
	result_object,
	CASE
		WHEN CAST(result_object::json -> 'ReviewItems' -> 0 -> 'Message' AS text) LIKE '%Account filing%' THEN result_object::json -> 'ReviewItems' -> 0 -> 'Message'
		WHEN CAST(result_object::json -> 'ReviewItems' -> 1 -> 'Message' AS text) LIKE '%Account filing%' THEN result_object::json -> 'ReviewItems' -> 1 -> 'Message'
		WHEN CAST(result_object::json -> 'ReviewItems' -> 2 -> 'Message' AS text) LIKE '%Account filing%' THEN result_object::json -> 'ReviewItems' -> 2 -> 'Message'
	END AS error_message
  FROM ecr) AS messages
WHERE ecr_report.call_id = messages.call_id
AND messages.result_object @> '{"ReviewItems":[{"Characteristic": {"ErrorCode": 8} }]}'
;

-- BR09


UPDATE ecr_report
SET br09 = messages.error_message
FROM
 (SELECT
	call_id,
	result_object,
	CASE
		WHEN CAST(result_object::json -> 'ReviewItems' -> 0 -> 'Message' AS text) LIKE '%Abnormal filing%' THEN result_object::json -> 'ReviewItems' -> 0 -> 'Message'
		WHEN CAST(result_object::json -> 'ReviewItems' -> 1 -> 'Message' AS text) LIKE '%Abnormal filing%' THEN result_object::json -> 'ReviewItems' -> 1 -> 'Message'
		WHEN CAST(result_object::json -> 'ReviewItems' -> 2 -> 'Message' AS text) LIKE '%Abnormal filing%' THEN result_object::json -> 'ReviewItems' -> 2 -> 'Message'
	END AS error_message
  FROM ecr) AS messages
WHERE ecr_report.call_id = messages.call_id
AND messages.result_object @> '{"ReviewItems":[{"Characteristic": {"ErrorCode": 9} }]}'
;

-- BR10


UPDATE ecr_report
SET br10 = messages.error_message
FROM
 (SELECT
	call_id,
	result_object,
	CASE
		WHEN CAST(result_object::json -> 'RecordedItems' -> 0 -> 'Message' AS text) LIKE '%Non UK holding company%' THEN result_object::json -> 'RecordedItems' -> 0 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 1 -> 'Message' AS text) LIKE '%Non UK holding company%' THEN result_object::json -> 'RecordedItems' -> 1 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 2 -> 'Message' AS text) LIKE '%Non UK holding company%' THEN result_object::json -> 'RecordedItems' -> 2 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 3 -> 'Message' AS text) LIKE '%Non UK holding company%' THEN result_object::json -> 'RecordedItems' -> 3 -> 'Message'
	END AS error_message
  FROM ecr) AS messages
WHERE ecr_report.call_id = messages.call_id
AND messages.result_object @> '{"RecordedItems":[{"ErrorCode": 10}]}'
;

-- BR11


UPDATE ecr_report
SET br11 = messages.error_message
FROM
 (SELECT
	call_id,
	result_object,
	CASE
		WHEN CAST(result_object::json -> 'RecordedItems' -> 0 -> 'Message' AS text) LIKE '%Company registered office address%' THEN result_object::json -> 'RecordedItems' -> 0 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 1 -> 'Message' AS text) LIKE '%Company registered office address%' THEN result_object::json -> 'RecordedItems' -> 1 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 2 -> 'Message' AS text) LIKE '%Company registered office address%' THEN result_object::json -> 'RecordedItems' -> 2 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 3 -> 'Message' AS text) LIKE '%Company registered office address%' THEN result_object::json -> 'RecordedItems' -> 3 -> 'Message'
	END AS error_message
  FROM ecr) AS messages
WHERE ecr_report.call_id = messages.call_id
AND messages.result_object @> '{"RecordedItems":[{"ErrorCode": 11}]}'
;

-- BR12


UPDATE ecr_report
SET br12 = messages.error_message
FROM
 (SELECT
	call_id,
	result_object,
	CASE
		WHEN CAST(result_object::json -> 'RecordedItems' -> 0 -> 'Message' AS text) LIKE '%Multiple matches%' THEN result_object::json -> 'RecordedItems' -> 0 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 1 -> 'Message' AS text) LIKE '%Multiple matches%' THEN result_object::json -> 'RecordedItems' -> 1 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 2 -> 'Message' AS text) LIKE '%Multiple matches%' THEN result_object::json -> 'RecordedItems' -> 2 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 3 -> 'Message' AS text) LIKE '%Multiple matches%' THEN result_object::json -> 'RecordedItems' -> 3 -> 'Message'
	END AS error_message
  FROM ecr) AS messages
WHERE ecr_report.call_id = messages.call_id
AND messages.result_object @> '{"RecordedItems":[{"ErrorCode": 12}]}'
;

-- BR13


UPDATE ecr_report
SET br13 = messages.error_message
FROM
 (SELECT
	call_id,
	result_object,
	CASE
		WHEN CAST(result_object::json -> 'RecordedItems' -> 0 -> 'Message' AS text) LIKE '%Total agreements%' THEN result_object::json -> 'RecordedItems' -> 0 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 1 -> 'Message' AS text) LIKE '%Total agreements%' THEN result_object::json -> 'RecordedItems' -> 1 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 2 -> 'Message' AS text) LIKE '%Total agreements%' THEN result_object::json -> 'RecordedItems' -> 2 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 3 -> 'Message' AS text) LIKE '%Total agreements%' THEN result_object::json -> 'RecordedItems' -> 3 -> 'Message'
	END AS error_message
  FROM ecr) AS messages
WHERE ecr_report.call_id = messages.call_id
AND messages.result_object @> '{"RecordedItems":[{"ErrorCode": 13}]}'
;

-- BR15


UPDATE ecr_report
SET br15 = messages.error_message
FROM
 (SELECT
	call_id,
	result_object,
	CASE
		WHEN CAST(result_object::json -> 'RecordedItems' -> 0 -> 'Message' AS text) LIKE '%Ineligible score%' THEN result_object::json -> 'RecordedItems' -> 0 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 1 -> 'Message' AS text) LIKE '%Ineligible score%' THEN result_object::json -> 'RecordedItems' -> 1 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 2 -> 'Message' AS text) LIKE '%Ineligible score%' THEN result_object::json -> 'RecordedItems' -> 2 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 3 -> 'Message' AS text) LIKE '%Ineligible score%' THEN result_object::json -> 'RecordedItems' -> 3 -> 'Message'
	END AS error_message
  FROM ecr) AS messages
WHERE ecr_report.call_id = messages.call_id
AND messages.result_object @> '{"RecordedItems":[{"ErrorCode": 15}]}'
;

-- BR16


UPDATE ecr_report
SET br16 = messages.error_message
FROM
 (SELECT
	call_id,
	result_object,
	CASE
		WHEN CAST(result_object::json -> 'RecordedItems' -> 0 -> 'Message' AS text) LIKE '%Mortgage check%' THEN result_object::json -> 'RecordedItems' -> 0 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 1 -> 'Message' AS text) LIKE '%Mortgage check%' THEN result_object::json -> 'RecordedItems' -> 1 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 2 -> 'Message' AS text) LIKE '%Mortgage check%' THEN result_object::json -> 'RecordedItems' -> 2 -> 'Message'
		WHEN CAST(result_object::json -> 'RecordedItems' -> 3 -> 'Message' AS text) LIKE '%Mortgage check%' THEN result_object::json -> 'RecordedItems' -> 3 -> 'Message'
	END AS error_message
  FROM ecr) AS messages
WHERE ecr_report.call_id = messages.call_id
AND messages.result_object @> '{"RecordedItems":[{"ErrorCode": 16}]}'
;

-- Finally, the report!

SELECT * FROM ecr_report