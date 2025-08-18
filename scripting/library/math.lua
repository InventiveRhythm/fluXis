---@meta

---@class mathf
---@field pi number
---@field tau number
---@field e number
mathf = {}

---@param d number
---@return number
---@nodiscard
function mathf:abs(d) end

---@param value number
---@param min number
---@param max number
---@return number
---@nodiscard
function mathf:clamp(value, min, max) end

---@param a number
---@param b number
---@return number
---@nodiscard
function mathf:min(a, b) end

---@param a number
---@param b number
---@return number
---@nodiscard
function mathf:max(a, b) end

---Returns an integer that indicates the sign of a number.
----1 value is less than zero |
---0 value is equal to zero |
---1 value is greater than zero
---@param d number
---@return number
---@nodiscard
function mathf:sign(d) end

---@param d number
---@return number
---@nodiscard
function mathf:floor(d) end

---Floors the input value only if it meets or greater than the threshold.
---Otherwise returns the original value unchanged.
---@param d number the input
---@param threshold number The minimum value at which flooring will be applied. Values below this threshold remain unchanged.
---@return number
---@nodiscard
function mathf:floort(d, threshold) end

---@param d number
---@return number
---@nodiscard
function mathf:ceil(d) end

---Ceils the input value only if it is less than or equal to the specified threshold.
---Otherwise returns the original value unchanged.
---@param d number the input
---@param threshold number The maximum value at which ceiling will be applied. Values above this threshold remain unchanged.
---@return number
---@nodiscard
function mathf:ceilt(d, threshold) end

---@param d number
---@return number
---@nodiscard
function mathf:round(d) end

---Rounds the input value only if it is within the specified threshold distance from a whole number.
---Otherwise returns the original value unchanged.
---Useful for rounding values that are "close enough" to whole numbers while leaving others unchanged.
---@param d number the input
---@param threshold number The maximum allowable distance from a whole number for rounding to be applied.
---@return number
---@nodiscard
function mathf:roundt(d, threshold) end

---@param d number
---@return number
---@nodiscard
function mathf:sqrt(d) end

---@param x number
---@param y number
---@return number
---@nodiscard
function mathf:pow(x, y) end

---Returns e raised to the specified power.
---@param d number
---@return number
---@nodiscard
function mathf:exp(d) end

---Returns the natural logarithm (base e).
---@param d number
---@return number
---@nodiscard
function mathf:log(d) end

---@param d number
---@return number
---@nodiscard
function mathf:log10(d) end

---Linearly interpolates between two values by a given factor.
---@param a number
---@param b number
---@param t number
---@return number
---@nodiscard
function mathf:lerp(a, b, t) end

---Returns the sine of an angle.
---@param radians number The angle in radians.
---@return number
---@nodiscard
function mathf:sin(radians) end

---Returns the cosine of an angle.
---@param radians number The angle in radians.
---@return number
---@nodiscard
function mathf:cos(radians) end

---Returns the tangent of an angle.
---@param radians number The angle in radians.
---@return number
---@nodiscard
function mathf:tan(radians) end

---Returns the angle whose sine is the specified number.
---@param value number A number between -1 and 1.
---@return number # Angle in radians between -π/2 and π/2.
---@nodiscard
function mathf:asin(value) end

---Returns the angle whose cosine is the specified number.
---@param value number A number between -1 and 1.
---@return number # Angle in radians between 0 and π.
---@nodiscard
function mathf:acos(value) end

---Returns the angle whose tangent is the specified number.
---@param value number A real number.
---@return number # Angle in radians between -π/2 and π/2.
---@nodiscard
function mathf:atan(value) end

---Returns the angle from the y/x ratio, handling quadrants.
---@param x number The x coordinate.
---@param y number The y coordinate.
---@return number # Angle in radians between -π and π.
---@nodiscard
function mathf:atan2(x, y) end

---Converts from radians to degrees.
---@param radians number Angle in radians.
---@return number # Angle in degrees.
---@nodiscard
function mathf:deg(radians) end

---Converts from degrees to radians.
---@param degrees number Angle in degrees.
---@return number # Angle in radians.
---@nodiscard
function mathf:rad(degrees) end

---Returns the hyperbolic sine of the specified angle.
---@param radians number The angle in radians.
---@return number
---@nodiscard
function mathf:sinh(radians) end

---Returns the hyperbolic cosine of the specified angle.
---@param radians number The angle in radians.
---@return number
---@nodiscard
function mathf:cosh(radians) end

---Returns the hyperbolic tangent of the specified angle.
---@param radians number The angle in radians.
---@return number
---@nodiscard
function mathf:tanh(radians) end

---@param a Vector2
---@param b Vector2
---@return Vector2
---@nodiscard
function mathf:vecsub(a, b) end

---@param a Vector2
---@param b Vector2
---@return Vector2
---@nodiscard
function mathf:vecadd(a, b) end

---@param a Vector2
---@param scalar number
---@return Vector2
---@nodiscard
function mathf:vecmul(a, scalar) end

---@param a Vector2
---@param b Vector2
---@return number
---@nodiscard
function mathf:vecdot(a, b) end

---@param v Vector2
---@return number
---@nodiscard
function mathf:veclen(v) end

---@param v Vector2
---@return Vector2
---@nodiscard
function mathf:vecnorm(v) end

---Rotates a vector by a given angle in radians.
---@param v Vector2 the vector to rotate.
---@param angle number Angle in radians.
---@return Vector2 # a rotated vector.
---@nodiscard
function mathf:vecrotate(v, angle) end

---Rotates vector/point a around vector b by a given angle in radians.
---@param a Vector2 the vector to apply the rotation to.
---@param b Vector2 the vector to rotate around.
---@param angle number Angle in radians.
---@return Vector2 # a rotated vector.
---@nodiscard
function mathf:vecrotatearound(a, b, angle) end

---@param a Vector2
---@param b Vector2
---@param acceptableDifference number
---@return boolean
---@nodiscard
function mathf:vecequals(a, b, acceptableDifference) end

---Returns the angle of a vector in radians.
---@param v Vector2
---@return number # Angle in radians.
---@nodiscard
function mathf:vecangle(v) end

---Returns the angle of between two vectors/points in radians.
---@param a Vector2
---@param b Vector2
---@return number # Angle in radians.
---@nodiscard
function mathf:vecanglebetween(a, b) end

---@param a Vector2
---@param b Vector2
---@return number
---@nodiscard
function mathf:vecdist(a, b) end

---Checks if the vector is normalized.
---@param v Vector2
---@param acceptableDifference number
---@return boolean
---@nodiscard
function mathf:vecisnorm(v, acceptableDifference) end

---Checks if the vector is a zero vector.
---@param v Vector2
---@param acceptableDifference number
---@return boolean
---@nodiscard
function mathf:veciszero(v, acceptableDifference) end

---Returns a vector with all components set to its absolute values.
---@param v Vector2
---@return Vector2
---@nodiscard
function mathf:vecabs(v) end